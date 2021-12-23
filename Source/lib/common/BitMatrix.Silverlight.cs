/*
* Copyright 2007 ZXing authors
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
#if NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Media.Imaging;
#endif

using ZXing.OneD;
using ZXing.Rendering;

namespace ZXing.Common
{
    public sealed partial class BitMatrix
    {
        [Obsolete("Use BarcodeWriter instead")]
        public PixelData ToBitmap()
        {
            var renderer = new PixelDataRenderer();
            return renderer.Render(this, BarcodeFormat.EAN_8, string.Empty);
        }

        /// <summary>
        /// Converts this ByteMatrix to a black and white bitmap.
        /// </summary>
        /// <returns>A black and white bitmap converted from this ByteMatrix.</returns>
        [Obsolete("Use BarcodeWriter instead")]
        public PixelData ToBitmap(BarcodeFormat format, String content)
        {
            var renderer = new PixelDataRenderer();
            return renderer.Render(this, format, content);
        }
    }
}