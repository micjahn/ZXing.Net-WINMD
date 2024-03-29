﻿/*
 * Copyright 2013
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

namespace ZXing.Datamatrix.Encoder
{
    /// <summary>
    /// Enumeration for encodation types
    /// </summary>
    public enum Encodation : int
    {
        /// <summary>
        /// ASCII
        /// </summary>
        ASCII = 0,
        /// <summary>
        /// C40
        /// </summary>
        C40 = 1,
        /// <summary>
        /// TEXT
        /// </summary>
        TEXT = 2,
        /// <summary>
        /// X12
        /// </summary>
        X12 = 3,
        /// <summary>
        /// EDIFACT
        /// </summary>
        EDIFACT = 4,
        /// <summary>
        /// BASE256
        /// </summary>
        BASE256 = 5
    }
}