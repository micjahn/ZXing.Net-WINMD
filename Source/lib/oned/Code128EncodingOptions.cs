/*
 * Copyright 2013 ZXing.Net authors
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

namespace ZXing.OneD
{
    /// <summary>
    /// The class holds the available options for the Code128 1D Writer
    /// </summary>
    [Serializable]
    public sealed class Code128EncodingOptions : IEncodingOptions
    {
        /// <summary>
        /// if true, don't switch to codeset C for numbers
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [Category("Standard"), Description("If true, don't switch to codeset C for numbers.")]
#endif
        public bool ForceCodesetB
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.CODE128_FORCE_CODESET_B))
                {
                    return (bool)Hints[EncodeHintType.CODE128_FORCE_CODESET_B];
                }
                return false;
            }
            set
            {
                Hints[EncodeHintType.CODE128_FORCE_CODESET_B] = value;
            }
        }
        /// <summary>
        /// Forces which encoding will be used. Currently only used for Code-128 code sets (Type <see cref="System.String" />). Valid values are "A", "B", "C".
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [Category("Standard"), Description("Forces which encoding will be used. Valid values are \"A\", \"B\", \"C\".")]
#endif
        public int ForceCodeset
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.FORCE_CODE_SET))
                {
                    return (int)Hints[EncodeHintType.FORCE_CODE_SET];
                }
                return (int)Codesets.None;
            }
            set
            {
                if (value >= -1 && value < 2)
                    Hints[EncodeHintType.FORCE_CODE_SET] = value;
            }
        }

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
                    return (int)Hints[EncodeHintType.MARGIN];
                }
                return 0;
            }
            set
            {
                Hints[EncodeHintType.MARGIN] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Code128EncodingOptions"/> class.
        /// </summary>
        public Code128EncodingOptions()
        {
            Hints = new Dictionary<EncodeHintType, object>();
        }
    }
    /// <summary>
    /// avaiable codesets
    /// </summary>
    public enum Codesets : int
    {
        /// <summary>
        /// none specified
        /// </summary>
        None = -1,
        /// <summary>
        /// Codeset A
        /// </summary>
        A = 0,
        /// <summary>
        /// Codeset B
        /// </summary>
        B = 1,
        /// <summary>
        /// Codeset C
        /// </summary>
        C = 2
    }
}
