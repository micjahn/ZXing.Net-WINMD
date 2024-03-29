/*
* Copyright 2008 ZXing authors
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

namespace ZXing.Client.Result
{
    /// <summary>
    /// Represents a parsed result that encodes a telephone number.
    /// </summary>
    /// <author>Sean Owen</author>
    internal sealed class TelParsedResult : ParsedResult
    {
        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="telURI"></param>
        /// <param name="title"></param>
        public TelParsedResult(String number, String telURI, String title)
           : base(ParsedResultType.TEL)
        {
            Number = number;
            TelURI = telURI;
            Title = title;

            var result = new System.Text.StringBuilder(20);
            maybeAppend(number, result);
            maybeAppend(title, result);
            displayResultValue = result.ToString();
        }

        /// <summary>
        /// number
        /// </summary>
        public String Number { get; private set; }
        /// <summary>
        /// URI
        /// </summary>
        public String TelURI { get; private set; }
        /// <summary>
        /// title
        /// </summary>
        public String Title { get; private set; }
    }
}