/*
 * Copyright 2012 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if NETFX_CORE
using Windows.UI;
#elif SILVERLIGHT
using System.Windows.Media;
#elif UNITY
using UnityEngine;
#elif !(PORTABLE || NETSTANDARD)
using System.Drawing;
#endif

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a barcode into a Svg image
   /// </summary>
   public sealed class SvgRenderer : IBarcodeRendererSvg
   {
      internal const string DefaultFontName = "Arial";
      internal const int DefaultFontSize = 10;

#if !UNITY
#if (PORTABLE || NETSTANDARD)
      /// <summary>
      /// represents a color value
      /// </summary>
      public struct Color
      {
         /// <summary>
         /// color black
         /// </summary>
         public static Color Black = new Color(255, 0, 0, 0);
         /// <summary>
         /// color white
         /// </summary>
         public static Color White = new Color(255, 255, 255, 255);

         /// <summary>
         /// alpha channel
         /// </summary>
         public byte A;
         /// <summary>
         /// red channel
         /// </summary>
         public byte R;
         /// <summary>
         /// green channel
         /// </summary>
         public byte G;
         /// <summary>
         /// blur channel
         /// </summary>
         public byte B;

         /// <summary>
         /// initializing constructor
         /// </summary>
         public Color(int color)
         {
            A = (byte)((color & 0xFF000000) >> 24);
            R = (byte)((color & 0x00FF0000) >> 16);
            G = (byte)((color & 0x0000FF00) >> 8);
            B = (byte)((color & 0x000000FF));
         }

         /// <summary>
         /// initializing constructor
         /// </summary>
         public Color(byte alpha, byte red, byte green, byte blue)
         {
            A = alpha;
            R = red;
            G = green;
            B = blue;
         }
      }
#endif
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      [CLSCompliant(false)]
      public Color Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      [CLSCompliant(false)]
      public Color Background { get; set; }
#else
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      [System.CLSCompliant(false)]
      public Color32 Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      [System.CLSCompliant(false)]
      public Color32 Background { get; set; }
#endif

      /// <summary>
      /// Gets or sets the font family name
      /// </summary>
      /// <value>The font family name.</value>
      public string FontName { get; set; }

      /// <summary>
      /// Gets or sets the font size in pixel
      /// </summary>
      /// <value>The font size in pixel.</value>
      public int FontSize { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="SvgRenderer"/> class.
      /// </summary>
      public SvgRenderer()
      {
#if NETFX_CORE || SILVERLIGHT
         Foreground = Colors.Black;
         Background = Colors.White;
#elif UNITY
         Foreground = new Color32(0, 0, 0, 255);
         Background = new Color32(255, 255, 255, 255);
#else
         Foreground = Color.Black;
         Background = Color.White;
#endif
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      public SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var result = new SvgImage(matrix.Width, matrix.Height);

         Create(result, matrix, format, content, options);

         return result;
      }

      private void Create(SvgImage image, BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         if (matrix == null)
            return;

         const int spaceBetweenMatrixAndText = 3;
         int width = matrix.Width;
         int height = matrix.Height;
         var outputContent = (options == null || !options.PureBarcode) &&
                    !String.IsNullOrEmpty(content) &&
                    (format == BarcodeFormat.CODE_39 ||
                     format == BarcodeFormat.CODE_93 ||
                     format == BarcodeFormat.CODE_128 ||
                     format == BarcodeFormat.EAN_13 ||
                     format == BarcodeFormat.EAN_8 ||
                     format == BarcodeFormat.CODABAR ||
                     format == BarcodeFormat.ITF ||
                     format == BarcodeFormat.UPC_A ||
                     format == BarcodeFormat.UPC_E ||
                     format == BarcodeFormat.MSI ||
                     format == BarcodeFormat.PLESSEY);

         if (outputContent)
         {
            var fontSize = FontSize < 1 ? DefaultFontSize : FontSize;
            height += fontSize + spaceBetweenMatrixAndText;
         }

         image.AddHeader();
         image.AddTag(0, 0, width, height, Background, Foreground);
         AppendDarkCell(image, matrix, 0, 0);

         if (outputContent)
         {
            var fontName = String.IsNullOrEmpty(FontName) ? DefaultFontName : FontName;
            var fontSize = FontSize < 1 ? DefaultFontSize : FontSize;

            content = ModifyContentDependingOnBarcodeFormat(format, content);

            image.AddText(content, fontName, fontSize);
         }

         image.AddEnd();
      }

      private string ModifyContentDependingOnBarcodeFormat(BarcodeFormat format, string content)
      {
         switch (format)
         {
            case BarcodeFormat.EAN_8:
               if (content.Length < 8)
                  content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
               content = content.Insert(4, "   ");
               break;
            case BarcodeFormat.EAN_13:
               if (content.Length < 13)
                  content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
               content = content.Insert(7, "   ");
               content = content.Insert(1, "   ");
               break;
         }

         return content;
      }

      private static void AppendDarkCell(SvgImage image, BitMatrix matrix, int offsetX, int offSetY)
      {
         if (matrix == null)
            return;

         int width = matrix.Width;
         int height = matrix.Height;
         var processed = new BitMatrix(width, height);
         bool currentIsBlack = false;
         int startPosX = 0;
         int startPosY = 0;
         for (int x = 0; x < width; x++)
         {
            int endPosX;
            for (int y = 0; y < height; y++)
            {
               if (processed[x, y])
                  continue;

               processed[x, y] = true;

               if (matrix[x, y])
               {
                  if (!currentIsBlack)
                  {
                     startPosX = x;
                     startPosY = y;
                     currentIsBlack = true;
                  }
               }
               else
               {
                  if (currentIsBlack)
                  {
                     FindMaximumRectangle(matrix, processed, startPosX, startPosY, y, out endPosX);
                     image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, y - startPosY);
                     currentIsBlack = false;
                  }
               }
            }
            if (currentIsBlack)
            {
               FindMaximumRectangle(matrix, processed, startPosX, startPosY, height, out endPosX);
               image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, height - startPosY);
               currentIsBlack = false;
            }
         }
      }

      private static void FindMaximumRectangle(BitMatrix matrix, BitMatrix processed, int startPosX, int startPosY, int endPosY, out int endPosX)
      {
         endPosX = startPosX;

         for (int x = startPosX + 1; x < matrix.Width; x++)
         {
            for (int y = startPosY; y < endPosY; y++)
            {
               if (!matrix[x, y])
               {
                  return;
               }
            }
            endPosX = x;
            for (int y = startPosY; y < endPosY; y++)
            {
               processed[x, y] = true;
            }
         }
      }
   }
}