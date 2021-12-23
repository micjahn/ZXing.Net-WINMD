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

namespace ZXing.Aztec
{
    /// <summary>
    /// The class holds the available options for the <see cref="AztecWriter" />
    /// </summary>
    [Serializable]
    public sealed class AztecEncodingOptions : IEncodingOptions
    {
        /// <summary>
        /// Gets the data container for all options
        /// </summary>
        [Browsable(false)]
        public IDictionary<EncodeHintType, object> Hints { get; private set; }

        /// <summary>
        /// Representing the minimal percentage of error correction words. 
        /// Note: an Aztec symbol should have a minimum of 25% EC words.
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("The minimal percentage of error correction words (> 25%).")]
#endif
        public int? ErrorCorrection
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
                {
                    return (int)Hints[EncodeHintType.ERROR_CORRECTION];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
                        Hints.Remove(EncodeHintType.ERROR_CORRECTION);
                }
                else
                {
                    Hints[EncodeHintType.ERROR_CORRECTION] = value;
                }
            }
        }

        /// <summary>
        /// Specifies the required number of layers for an Aztec code:
        /// a negative number (-1, -2, -3, -4) specifies a compact Aztec code
        /// 0 indicates to use the minimum number of layers (the default)
        /// a positive number (1, 2, .. 32) specifies a normal (non-compact) Aztec code
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("The required number of layers for an Aztec code" +
                                                             " -1 to -4 specify a compact code, 0 indicates to use the minimum number of layers and" +
                                                             " 1 to 32 specify a normal (non-compact) Aztec code.")]
#endif
        public int? Layers
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
                {
                    return (int)Hints[EncodeHintType.AZTEC_LAYERS];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
                        Hints.Remove(EncodeHintType.AZTEC_LAYERS);
                }
                else
                {
                    Hints[EncodeHintType.AZTEC_LAYERS] = value;
                }
            }
        }


        /// <summary>
        /// Specifies what character encoding to use where applicable (type <see cref="String"/>)
        /// </summary>
#if !NETSTANDARD && !NETFX_CORE && !WindowsCE && !SILVERLIGHT && !PORTABLE && !UNITY
        [CategoryAttribute("Standard"), DescriptionAttribute("Specifies what character encoding to use where applicable.")]
#endif
        public string CharacterSet
        {
            get
            {
                if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
                {
                    return (string)Hints[EncodeHintType.CHARACTER_SET];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
                        Hints.Remove(EncodeHintType.CHARACTER_SET);
                }
                else
                {
                    Hints[EncodeHintType.CHARACTER_SET] = value;
                }
            }
        }
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
        /// Initializes a new instance of the <see cref="AztecEncodingOptions"/> class.
        /// </summary>
        public AztecEncodingOptions()
        {
            Hints = new Dictionary<EncodeHintType, object>();
        }
    }
}
