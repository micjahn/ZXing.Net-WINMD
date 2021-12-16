/*
 * Copyright 2009 ZXing authors
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
using ZXing.Common;

namespace ZXing.OneD
{
    /// <summary>
    /// This object renders an EAN13 code as a <see cref="BitMatrix"/>.
    /// <author>aripollak@gmail.com (Ari Pollak)</author>
    /// </summary>
   internal sealed class EAN13Writer : UPCEANWriter
    {
        private const int CODE_WIDTH = 3 + // start guard
            (7 * 6) + // left bars
            5 + // middle guard
            (7 * 6) + // right bars
            3; // end guard

        private static readonly IList<BarcodeFormat> supportedWriteFormats = new List<BarcodeFormat> { BarcodeFormat.EAN_13 };

        protected override IList<BarcodeFormat> SupportedWriteFormats
        {
            get { return supportedWriteFormats; }
        }

        /// <summary>
        /// Encode the contents to byte array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public override bool[] encode(String contents)
        {
            int length = contents.Length;
            switch (length)
            {
                case 12:
                    // No check digit present, calculate it and add it
                    var check = UPCEANReader.getStandardUPCEANChecksum(contents);
                    if (check == null)
                    {
                        throw new ArgumentException("Checksum can't be calculated");
                    }
                    contents += check.Value;
                    break;
                case 13:
                    try
                    {
                        if (!UPCEANReader.checkStandardUPCEANChecksum(contents))
                        {
                            throw new ArgumentException("Contents do not pass checksum");
                        }
                    }
                    catch (FormatException ignored)
                    {
                        throw new ArgumentException("Illegal contents", ignored);
                    }
                    break;
                default:
                    throw new ArgumentException("Requested contents should be 12 (without checksum digit) or 13 digits long, but got " + contents.Length);
            }

            checkNumeric(contents);

            int firstDigit = Int32.Parse(contents.Substring(0, 1));
            int parities = EAN13Reader.FIRST_DIGIT_ENCODINGS[firstDigit];
            var result = new bool[CODE_WIDTH];
            int pos = 0;

            pos += appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

            // See EAN13Reader for a description of how the first digit & left bars are encoded
            for (int i = 1; i <= 6; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, 1));
                if ((parities >> (6 - i) & 1) == 1)
                {
                    digit += 10;
                }
                pos += appendPattern(result, pos, UPCEANReader.L_AND_G_PATTERNS[digit], false);
            }

            pos += appendPattern(result, pos, UPCEANReader.MIDDLE_PATTERN, false);

            for (int i = 7; i <= 12; i++)
            {
                int digit = Int32.Parse(contents.Substring(i, 1));
                pos += appendPattern(result, pos, UPCEANReader.L_PATTERNS[digit], true);
            }
            appendPattern(result, pos, UPCEANReader.START_END_PATTERN, true);

            return result;
        }
    }
}
