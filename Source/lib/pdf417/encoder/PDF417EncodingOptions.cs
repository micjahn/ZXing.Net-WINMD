﻿/*
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
using System.ComponentModel;
using ZXing.Common;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417
{
   /// <summary>
   /// The class holds the available options for the <see cref="PDF417Writer" />
   /// </summary>
   [Serializable]
   public class PDF417EncodingOptions : IEncodingOptions
   {
      /// <summary>
      /// Gets the data container for all options
      /// </summary>
      [Browsable(false)]
      public IDictionary<EncodeHintType, object> Hints { get; private set; }

      /// <summary>
      /// Specifies the height of the barcode image
      /// </summary>
      public int Height
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.HEIGHT))
            {
               return (int)Hints[EncodeHintType.HEIGHT];
            }
            return 0;
         }
         set
         {
            Hints[EncodeHintType.HEIGHT] = value;
         }
      }

      /// <summary>
      /// Specifies the width of the barcode image
      /// </summary>
      public int Width
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.WIDTH))
            {
               return (int)Hints[EncodeHintType.WIDTH];
            }
            return 0;
         }
         set
         {
            Hints[EncodeHintType.WIDTH] = value;
         }
      }

      /// <summary>
      /// Don't put the content string into the output image.
      /// </summary>
      public bool PureBarcode
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PURE_BARCODE))
            {
               return (bool)Hints[EncodeHintType.PURE_BARCODE];
            }
            return false;
         }
         set
         {
            Hints[EncodeHintType.PURE_BARCODE] = value;
         }
      }

      /// <summary>
      /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary
      /// by format; for example it controls margin before and after the barcode horizontally for
      /// most 1D formats.
      /// </summary>
      public int Margin
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.MARGIN))
            {
               return (int) Hints[EncodeHintType.MARGIN];
            }
            return 0;
         }
         set
         {
            Hints[EncodeHintType.MARGIN] = value;
         }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="PDF417EncodingOptions"/> class.
      /// </summary>
      public PDF417EncodingOptions()
      {
         Hints = new Dictionary<EncodeHintType, object>();
      }
      
      /// <summary>
      /// Specifies whether to use compact mode for PDF417 (type <see cref="bool" />).
      /// </summary>
      public bool Compact
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_COMPACT))
            {
               return (bool)Hints[EncodeHintType.PDF417_COMPACT];
            }
            return false;
         }
         set
         {
            Hints[EncodeHintType.PDF417_COMPACT] = value;
         }
      }

      /// <summary>
      /// Specifies what compaction mode to use for PDF417 (type
      /// <see cref="Compaction" />).
      /// </summary>
      public Compaction Compaction
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_COMPACTION))
            {
               return (Compaction)Hints[EncodeHintType.PDF417_COMPACTION];
            }
            return Compaction.AUTO;
         }
         set
         {
            Hints[EncodeHintType.PDF417_COMPACTION] = value;
         }
      }

      /// <summary>
      /// Specifies the minimum and maximum number of rows and columns for PDF417 (type
      /// <see cref="Dimensions" />).
      /// </summary>
      public Dimensions Dimensions
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_DIMENSIONS))
            {
               return (Dimensions)Hints[EncodeHintType.PDF417_DIMENSIONS];
            }
            return null;
         }
         set
         {
            Hints[EncodeHintType.PDF417_DIMENSIONS] = value;
         }
      }
   }
}
