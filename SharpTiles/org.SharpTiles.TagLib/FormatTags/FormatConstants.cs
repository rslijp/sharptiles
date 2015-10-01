/*
 * SharpTiles, R.Z. Slijp(2008), www.sharptiles.org
 *
 * This file is part of SharpTiles.
 * 
 * SharpTiles is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * SharpTiles is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with SharpTiles.  If not, see <http://www.gnu.org/licenses/>.
 */
 using System.Collections.Generic;
using System.Globalization;

namespace org.SharpTiles.Tags.FormatTags
{
    public class FormatConstants
    {
        public const string BUNDLE = "I18NBundle";

        public const string CURRENCY_PATTERN = "c";
        public const string LOCALE = "I18NLocale";
        public const string NUMBER_PATTERN = "n";
        public const string PERCENTAGE_PATTERN = "p";

        public static IDictionary<NumberType, string> NUMBERPATTERNS = new Dictionary<NumberType, string>();
        public static IDictionary<NumberType, NumberStyles> NUMBERSTYLES = new Dictionary<NumberType, NumberStyles>();

        static FormatConstants()
        {
            NUMBERPATTERNS.Add(NumberType.Number, NUMBER_PATTERN);
            NUMBERPATTERNS.Add(NumberType.Currency, CURRENCY_PATTERN);
            NUMBERPATTERNS.Add(NumberType.Percentage, PERCENTAGE_PATTERN);


            NUMBERSTYLES.Add(NumberType.Number, NumberStyles.Number);
            NUMBERSTYLES.Add(NumberType.Currency, NumberStyles.Currency);
            NUMBERSTYLES.Add(NumberType.Percentage, NumberStyles.Number);
        }
    }
}
