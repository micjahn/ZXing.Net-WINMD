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
using System.Text;
using Windows.UI;
using ZXing.Common;

namespace ZXing.Rendering
{
   /// <summary>
   /// Interface for a class to convert a BitMatrix to an output image format
   /// </summary>
   public interface IBarcodeRendererSvg
   {
      /// <summary>
      /// Renders the specified matrix to its graphically representation
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The encoded content of the barcode which should be included in the image.
      /// That can be the numbers below a 1D barcode or something other.</param>
      /// <returns></returns>
      SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content);

      /// <summary>
      /// Renders the specified matrix to its graphically representation
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The encoded content of the barcode which should be included in the image.
      /// That can be the numbers below a 1D barcode or something other.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options);
   }

   /// <summary>
   /// Represents a barcode as a Svg image
   /// </summary>
   public sealed class SvgImage
   {
      private readonly StringBuilder content;

      /// <summary>
      /// Gets or sets the content.
      /// </summary>
      /// <value>
      /// The content.
      /// </value>
      public String Content
      {
         get { return content.ToString(); }
         set { content.Length = 0; if (value != null) content.Append(value); }
      }

      /// <summary>
      /// The original height of the bitmatrix for the barcode
      /// </summary>
      public int Height { get; set; }

      /// <summary>
      /// The original width of the bitmatrix for the barcode
      /// </summary>
      public int Width { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="SvgImage"/> class.
      /// </summary>
      public SvgImage()
      {
         content = new StringBuilder();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SvgImage"/> class.
      /// </summary>
      public SvgImage(int width, int height)
      {
         content = new StringBuilder();
         Width = width;
         Height = height;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SvgImage"/> class.
      /// </summary>
      /// <param name="content">The content.</param>
      public SvgImage(string content)
      {
         this.content = new StringBuilder(content);
      }

      /// <summary>
      /// Gives the XML representation of the SVG image
      /// </summary>
      public override string ToString()
      {
         return content.ToString();
      }

      internal void AddHeader()
      {
         content.Append("<?xml version=\"1.0\" standalone=\"no\"?>");
         content.Append(@"<!-- Created with ZXing.Net (http://zxingnet.codeplex.com/) -->");
         content.Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
      }

      internal void AddEnd()
      {
         content.Append("</svg>");
      }

      internal void AddTag(int displaysizeX, int displaysizeY, int viewboxSizeX, int viewboxSizeY, Color background, Color fill)
      {

         if (displaysizeX <= 0 || displaysizeY <= 0)
            content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" shape-rendering=\"crispEdges\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6}>",
                viewboxSizeX,
                viewboxSizeY,
                GetColorRgb(background),
                ConvertAlpha(background),
                GetColorRgb(fill),
                ConvertAlpha(fill),
                GetBackgroundStyle(background)
                ));
         else
            content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" shape-rendering=\"crispEdges\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6} width=\"{7}\" height=\"{8}\">",
                viewboxSizeX,
                viewboxSizeY,
                GetColorRgb(background),
                ConvertAlpha(background),
                GetColorRgb(fill),
                ConvertAlpha(fill),
                GetBackgroundStyle(background),
                displaysizeX,
                displaysizeY));
      }

      internal void AddText(string text, string fontName, int fontSize)
      {
         content.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
            "<text x=\"50%\" y=\"98%\" style=\"font-family: {0}; font-size: {1}px\" text-anchor=\"middle\">{2}</text>",
            fontName, fontSize, text);
      }

      internal void AddRec(int posX, int posY, int width, int height)
      {
         content.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\"/>", posX, posY, width, height);
      }

#if !UNITY
      internal static double ConvertAlpha(Color alpha)
      {
         return Math.Round((((double)alpha.A) / (double)255), 2);
      }

      internal static string GetBackgroundStyle(Color color)
      {
         double alpha = ConvertAlpha(color);
         return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"",
             color.R, color.G, color.B, alpha);
      }

      internal static string GetColorRgb(Color color)
      {
         return color.R + "," + color.G + "," + color.B;
      }

      internal static string GetColorRgba(Color color)
      {
         double alpha = ConvertAlpha(color);
         return color.R + "," + color.G + "," + color.B + "," + alpha;
      }
#else
         internal static double ConvertAlpha(Color32 alpha)
         {
            return Math.Round((((double)alpha.a) / (double)255), 2);
         }

         internal static string GetBackgroundStyle(Color32 color)
         {
            double alpha = ConvertAlpha(color);
            return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"",
                color.r, color.g, color.b, alpha);
         }

         internal static string GetColorRgb(Color32 color)
         {
            return color.r + "," + color.g + "," + color.b;
         }

         internal static string GetColorRgba(Color32 color)
         {
            double alpha = ConvertAlpha(color);
            return color.r + "," + color.g + "," + color.b + "," + alpha;
         }
#endif
   }
}
