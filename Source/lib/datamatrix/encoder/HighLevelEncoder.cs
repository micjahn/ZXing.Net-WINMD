/*
 * Copyright 2006-2007 Jeremias Maerki.
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

namespace ZXing.Datamatrix.Encoder
{
    /// <summary>
    /// DataMatrix ECC 200 data encoder following the algorithm described in ISO/IEC 16022:200(E) in
    /// annex S.
    /// </summary>
    internal static class HighLevelEncoder
    {
        /// <summary>
        /// Padding character
        /// </summary>
        public const char PAD = (char)129;
        /// <summary>
        /// mode latch to C40 encodation mode
        /// </summary>
        public const char LATCH_TO_C40 = (char)230;
        /// <summary>
        /// mode latch to Base 256 encodation mode
        /// </summary>
        public const char LATCH_TO_BASE256 = (char)231;
        /// <summary>
        /// FNC1 Codeword
        /// </summary>
        public const char FNC1 = (char)232;
        /// <summary>
        /// Structured Append Codeword
        /// </summary>
        public const char STRUCTURED_APPEND = (char)233;
        /// <summary>
        /// Reader Programming
        /// </summary>
        public const char READER_PROGRAMMING = (char)234;
        /// <summary>
        /// Upper Shift
        /// </summary>
        public const char UPPER_SHIFT = (char)235;
        /// <summary>
        /// 05 Macro
        /// </summary>
        public const char MACRO_05 = (char)236;
        /// <summary>
        /// 06 Macro
        /// </summary>
        public const char MACRO_06 = (char)237;
        /// <summary>
        /// mode latch to ANSI X.12 encodation mode
        /// </summary>
        public const char LATCH_TO_ANSIX12 = (char)238;
        /// <summary>
        /// mode latch to Text encodation mode
        /// </summary>
        public const char LATCH_TO_TEXT = (char)239;
        /// <summary>
        /// mode latch to EDIFACT encodation mode
        /// </summary>
        public const char LATCH_TO_EDIFACT = (char)240;
        /// <summary>
        /// ECI character (Extended Channel Interpretation)
        /// </summary>
        public const char ECI = (char)241;

        /// <summary>
        /// Unlatch from C40 encodation
        /// </summary>
        public const char C40_UNLATCH = (char)254;
        /// <summary>
        /// Unlatch from X12 encodation
        /// </summary>
        public const char X12_UNLATCH = (char)254;

        /// <summary>
        /// 05 Macro header
        /// </summary>
        public const String MACRO_05_HEADER = "[)>\u001E05\u001D";
        /// <summary>
        /// 06 Macro header
        /// </summary>
        public const String MACRO_06_HEADER = "[)>\u001E06\u001D";
        /// <summary>
        /// Macro trailer
        /// </summary>
        public const String MACRO_TRAILER = "\u001E\u0004";

        private static char randomize253State(int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 253) + 1;
            int tempVariable = PAD + pseudoRandom;
            return (char)(tempVariable <= 254 ? tempVariable : tempVariable - 254);
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>the encoded message (the char values range from 0 to 255)</returns>
        public static String encodeHighLevel(String msg)
        {
            return encodeHighLevel(msg, SymbolShapeHint.FORCE_NONE, null, null, (int)Encodation.ASCII, false, null, false);
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="shape">requested shape. May be {@code SymbolShapeHint.FORCE_NONE},{@code SymbolShapeHint.FORCE_SQUARE} or {@code SymbolShapeHint.FORCE_RECTANGLE}.</param>
        /// <param name="minSize">the minimum symbol size constraint or null for no constraint</param>
        /// <param name="maxSize">the maximum symbol size constraint or null for no constraint</param>
        /// <param name="defaultEncodation">encoding mode to start with</param>
        /// <returns>the encoded message (the char values range from 0 to 255)</returns>
        public static String encodeHighLevel(String msg,
                                             SymbolShapeHint shape,
                                             Dimension minSize,
                                             Dimension maxSize,
                                             int defaultEncodation)
        {
            return encodeHighLevel(msg, shape, minSize, maxSize, defaultEncodation, false, null, false);
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="shape">requested shape. May be {@code SymbolShapeHint.FORCE_NONE},{@code SymbolShapeHint.FORCE_SQUARE} or {@code SymbolShapeHint.FORCE_RECTANGLE}.</param>
        /// <param name="minSize">the minimum symbol size constraint or null for no constraint</param>
        /// <param name="maxSize">the maximum symbol size constraint or null for no constraint</param>
        /// <param name="defaultEncodation">encoding mode to start with</param>
        /// <param name="forceC40">enforce C40 encoding</param>
        /// <param name="encoding"></param>
        /// <param name="disableEci"></param>
        /// <returns>the encoded message (the char values range from 0 to 255)</returns>
        public static String encodeHighLevel(String msg,
                                             SymbolShapeHint shape,
                                             Dimension minSize,
                                             Dimension maxSize,
                                             int defaultEncodation,
                                             bool forceC40,
                                             Encoding encoding,
                                             bool disableEci)
        {
            //the codewords 0..255 are encoded as Unicode characters
            C40Encoder c40Encoder = new C40Encoder();
            Encoder[] encoders =
               {
               new ASCIIEncoder(), c40Encoder, new TextEncoder(),
               new X12Encoder(), new EdifactEncoder(), new Base256Encoder()
            };

            var context = new EncoderContext(msg, encoding, disableEci);
            context.setSymbolShape(shape);
            context.setSizeConstraints(minSize, maxSize);

            if (msg.StartsWith(MACRO_05_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.writeCodeword(MACRO_05);
                context.setSkipAtEnd(2);
                context.Pos += MACRO_05_HEADER.Length;
            }
            else if (msg.StartsWith(MACRO_06_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.writeCodeword(MACRO_06);
                context.setSkipAtEnd(2);
                context.Pos += MACRO_06_HEADER.Length;
            }

            int encodingMode = defaultEncodation; //Default mode

            if (forceC40)
            {
                c40Encoder.encodeMaximal(context);
                encodingMode = context.NewEncoding;
                context.resetEncoderSignal();
            }

            switch (encodingMode)
            {
                case (int)Encodation.BASE256:
                    context.writeCodeword(HighLevelEncoder.LATCH_TO_BASE256);
                    break;
                case (int)Encodation.C40:
                    context.writeCodeword(HighLevelEncoder.LATCH_TO_C40);
                    break;
                case (int)Encodation.X12:
                    context.writeCodeword(HighLevelEncoder.LATCH_TO_ANSIX12);
                    break;
                case (int)Encodation.TEXT:
                    context.writeCodeword(HighLevelEncoder.LATCH_TO_TEXT);
                    break;
                case (int)Encodation.EDIFACT:
                    context.writeCodeword(HighLevelEncoder.LATCH_TO_EDIFACT);
                    break;
                case (int)Encodation.ASCII:
                    break;
                default:
                    throw new InvalidOperationException("Illegal mode: " + encodingMode);
            }
            while (context.HasMoreCharacters)
            {
                encoders[encodingMode].encode(context);
                if (context.NewEncoding >= 0)
                {
                    encodingMode = context.NewEncoding;
                    context.resetEncoderSignal();
                }
            }
            int len = context.Codewords.Length;
            context.updateSymbolInfo();
            int capacity = context.SymbolInfo.dataCapacity;
            if (len < capacity &&
                encodingMode != (int)Encodation.ASCII &&
                encodingMode != (int)Encodation.BASE256 &&
                encodingMode != (int)Encodation.EDIFACT)
            {
                context.writeCodeword('\u00fe'); //Unlatch (254)
            }
            //Padding
            StringBuilder codewords = context.Codewords;
            if (codewords.Length < capacity)
            {
                codewords.Append(PAD);
            }
            while (codewords.Length < capacity)
            {
                codewords.Append(randomize253State(codewords.Length + 1));
            }

            return context.Codewords.ToString();
        }

        internal static int lookAheadTest(String msg, int startpos, int currentMode)
        {
            int newMode = lookAheadTestIntern(msg, startpos, currentMode);
            if (currentMode == (int)Encodation.X12 && newMode == (int)Encodation.X12)
            {
                int endpos = Math.Min(startpos + 3, msg.Length);
                for (int i = startpos; i < endpos; i++)
                {
                    if (!isNativeX12(msg[i]))
                    {
                        return (int)Encodation.ASCII;
                    }
                }
            }
            else if (currentMode == (int)Encodation.EDIFACT && newMode == (int)Encodation.EDIFACT)
            {
                int endpos = Math.Min(startpos + 4, msg.Length);
                for (int i = startpos; i < endpos; i++)
                {
                    if (!isNativeEDIFACT(msg[i]))
                    {
                        return (int)Encodation.ASCII;
                    }
                }
            }
            return newMode;
        }

        internal static int lookAheadTestIntern(String msg, int startpos, int currentMode)
        {
            if (startpos >= msg.Length)
            {
                return currentMode;
            }
            float[] charCounts;
            //step J
            if (currentMode == (int)Encodation.ASCII)
            {
                charCounts = new[] { 0, 1, 1, 1, 1, 1.25f };
            }
            else
            {
                charCounts = new[] { 1, 2, 2, 2, 2, 2.25f };
                charCounts[currentMode] = 0;
            }

            var charsProcessed = 0;
            var mins = new byte[6];
            var intCharCounts = new int[6];
            while (true)
            {
                //step K
                if ((startpos + charsProcessed) == msg.Length)
                {
                    SupportClass.Fill(mins, (byte)0);
                    SupportClass.Fill(intCharCounts, 0);
                    var min = findMinimums(charCounts, intCharCounts, Int32.MaxValue, mins);
                    var minCount = getMinimumCount(mins);

                    if (intCharCounts[(int)Encodation.ASCII] == min)
                    {
                        return (int)Encodation.ASCII;
                    }
                    if (minCount == 1)
                    {
                        if (mins[(int)Encodation.BASE256] > 0)
                        {
                            return (int)Encodation.BASE256;
                        }
                        if (mins[(int)Encodation.EDIFACT] > 0)
                        {
                            return (int)Encodation.EDIFACT;
                        }
                        if (mins[(int)Encodation.TEXT] > 0)
                        {
                            return (int)Encodation.TEXT;
                        }
                        if (mins[(int)Encodation.X12] > 0)
                        {
                            return (int)Encodation.X12;
                        }
                    }
                    return (int)Encodation.C40;
                }

                char c = msg[startpos + charsProcessed];
                charsProcessed++;

                //step L
                if (isDigit(c))
                {
                    charCounts[(int)Encodation.ASCII] += 0.5f;
                }
                else if (isExtendedASCII(c))
                {
                    charCounts[(int)Encodation.ASCII] = (float)Math.Ceiling(charCounts[(int)Encodation.ASCII]);
                    charCounts[(int)Encodation.ASCII] += 2.0f;
                }
                else
                {
                    charCounts[(int)Encodation.ASCII] = (float)Math.Ceiling(charCounts[(int)Encodation.ASCII]);
                    charCounts[(int)Encodation.ASCII]++;
                }

                //step M
                if (isNativeC40(c))
                {
                    charCounts[(int)Encodation.C40] += 2.0f / 3.0f;
                }
                else if (isExtendedASCII(c))
                {
                    charCounts[(int)Encodation.C40] += 8.0f / 3.0f;
                }
                else
                {
                    charCounts[(int)Encodation.C40] += 4.0f / 3.0f;
                }

                //step N
                if (isNativeText(c))
                {
                    charCounts[(int)Encodation.TEXT] += 2.0f / 3.0f;
                }
                else if (isExtendedASCII(c))
                {
                    charCounts[(int)Encodation.TEXT] += 8.0f / 3.0f;
                }
                else
                {
                    charCounts[(int)Encodation.TEXT] += 4.0f / 3.0f;
                }

                //step O
                if (isNativeX12(c))
                {
                    charCounts[(int)Encodation.X12] += 2.0f / 3.0f;
                }
                else if (isExtendedASCII(c))
                {
                    charCounts[(int)Encodation.X12] += 13.0f / 3.0f;
                }
                else
                {
                    charCounts[(int)Encodation.X12] += 10.0f / 3.0f;
                }

                //step P
                if (isNativeEDIFACT(c))
                {
                    charCounts[(int)Encodation.EDIFACT] += 3.0f / 4.0f;
                }
                else if (isExtendedASCII(c))
                {
                    charCounts[(int)Encodation.EDIFACT] += 17.0f / 4.0f;
                }
                else
                {
                    charCounts[(int)Encodation.EDIFACT] += 13.0f / 4.0f;
                }

                // step Q
                if (isSpecialB256(c))
                {
                    charCounts[(int)Encodation.BASE256] += 4.0f;
                }
                else
                {
                    charCounts[(int)Encodation.BASE256]++;
                }

                //step R
                if (charsProcessed >= 4)
                {
                    SupportClass.Fill(mins, (byte)0);
                    SupportClass.Fill(intCharCounts, 0);
                    findMinimums(charCounts, intCharCounts, Int32.MaxValue, mins);

                    if (intCharCounts[(int)Encodation.ASCII] < min(intCharCounts[(int)Encodation.BASE256],
                          intCharCounts[(int)Encodation.C40], intCharCounts[(int)Encodation.TEXT], intCharCounts[(int)Encodation.X12],
                          intCharCounts[(int)Encodation.EDIFACT]))
                    {
                        return (int)Encodation.ASCII;
                    }
                    if (intCharCounts[(int)Encodation.BASE256] < intCharCounts[(int)Encodation.ASCII] ||
                          intCharCounts[(int)Encodation.BASE256] + 1 < min(intCharCounts[(int)Encodation.C40],
                          intCharCounts[(int)Encodation.TEXT], intCharCounts[(int)Encodation.X12], intCharCounts[(int)Encodation.EDIFACT]))
                    {
                        return (int)Encodation.BASE256;
                    }
                    if (intCharCounts[(int)Encodation.EDIFACT] + 1 < min(intCharCounts[(int)Encodation.BASE256],
                          intCharCounts[(int)Encodation.C40], intCharCounts[(int)Encodation.TEXT], intCharCounts[(int)Encodation.X12],
                          intCharCounts[(int)Encodation.ASCII]))
                    {
                        return (int)Encodation.EDIFACT;
                    }
                    if (intCharCounts[(int)Encodation.TEXT] + 1 < min(intCharCounts[(int)Encodation.BASE256],
                          intCharCounts[(int)Encodation.C40], intCharCounts[(int)Encodation.EDIFACT], intCharCounts[(int)Encodation.X12],
                          intCharCounts[(int)Encodation.ASCII]))
                    {
                        return (int)Encodation.TEXT;
                    }
                    if (intCharCounts[(int)Encodation.X12] + 1 < min(intCharCounts[(int)Encodation.BASE256],
                          intCharCounts[(int)Encodation.C40], intCharCounts[(int)Encodation.EDIFACT], intCharCounts[(int)Encodation.TEXT],
                          intCharCounts[(int)Encodation.ASCII]))
                    {
                        return (int)Encodation.X12;
                    }
                    if (intCharCounts[(int)Encodation.C40] + 1 < min(intCharCounts[(int)Encodation.ASCII],
                          intCharCounts[(int)Encodation.BASE256], intCharCounts[(int)Encodation.EDIFACT], intCharCounts[(int)Encodation.TEXT]))
                    {
                        if (intCharCounts[(int)Encodation.C40] < intCharCounts[(int)Encodation.X12])
                        {
                            return (int)Encodation.C40;
                        }
                        if (intCharCounts[(int)Encodation.C40] == intCharCounts[(int)Encodation.X12])
                        {
                            int p = startpos + charsProcessed + 1;
                            while (p < msg.Length)
                            {
                                char tc = msg[p];
                                if (isX12TermSep(tc))
                                {
                                    return (int)Encodation.X12;
                                }
                                if (!isNativeX12(tc))
                                {
                                    break;
                                }
                                p++;
                            }
                            return (int)Encodation.C40;
                        }
                    }
                }
            }
        }

        private static int min(int f1, int f2, int f3, int f4, int f5)
        {
            return Math.Min(min(f1, f2, f3, f4), f5);
        }

        private static int min(int f1, int f2, int f3, int f4)
        {
            return Math.Min(f1, Math.Min(f2, Math.Min(f3, f4)));
        }

        private static int findMinimums(float[] charCounts, int[] intCharCounts, int min, byte[] mins)
        {
            for (int i = 0; i < 6; i++)
            {
                int current = (intCharCounts[i] = (int)Math.Ceiling(charCounts[i]));
                if (min > current)
                {
                    min = current;
                    SupportClass.Fill(mins, (byte)0);
                }
                if (min == current)
                {
                    mins[i]++;
                }
            }
            return min;
        }

        private static int getMinimumCount(byte[] mins)
        {
            int minCount = 0;
            for (int i = 0; i < 6; i++)
            {
                minCount += mins[i];
            }
            return minCount;
        }

        internal static bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        internal static bool isExtendedASCII(char ch)
        {
            return ch >= 128 && ch <= 255;
        }

        internal static bool isNativeC40(char ch)
        {
            return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
        }

        internal static bool isNativeText(char ch)
        {
            return (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'z') || ch == 0x001d;
        }

        internal static bool isNativeX12(char ch)
        {
            return isX12TermSep(ch) || (ch == ' ') || (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z');
        }

        internal static bool isX12TermSep(char ch)
        {
            return (ch == '\r') //CR
                || (ch == '*')
                || (ch == '>');
        }

        internal static bool isNativeEDIFACT(char ch)
        {
            return ch >= ' ' && ch <= '^';
        }

        internal static bool isSpecialB256(char ch)
        {
            return false; //TODO NOT IMPLEMENTED YET!!!
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using numeric compaction.
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="startpos">the start position within the message</param>
        /// <returns>the requested character count</returns>
        public static int determineConsecutiveDigitCount(String msg, int startpos)
        {
            int len = msg.Length;
            int idx = startpos;
            while (idx < len && isDigit(msg[idx]))
            {
                idx++;
            }
            return idx - startpos;
        }

        internal static void illegalCharacter(char c)
        {
            throw new ArgumentException(String.Format("Illegal character: {0} (0x{1:X})", c, (int)c));
        }
    }
}