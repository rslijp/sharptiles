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
 using System;
using System.Linq;
using System.Reflection;
 using org.SharpTiles.Documentation.DocumentationAttributes;
 using org.SharpTiles.Tags;
using org.SharpTiles.Tags.DefaultPropertyValues;

namespace org.SharpTiles.Documentation
{
    public class PropertyDocumentation : IDescriptionElement
    {
        private readonly ResourceKeyStack _messagePath;
        private readonly PropertyInfo _property;
        private bool _required;
        private string _default;

        public PropertyDocumentation(ResourceKeyStack messagePath, PropertyInfo property)
        {
            _property = property;
            _messagePath = messagePath.BranchFor(property);
            DescriptionAttribute.Harvast(_messagePath, property);
            IsRequired(property);
            DetermineDefault(property);
        }

       

        public bool Required
        {
            get { return _required; }
        }

        public string Default
        {
            get { return _default; }
        }

        #region IDescriptionElement Members

        public string DescriptionKey
        {
            get { return _messagePath.Description; }
        }

        public string Name
        {
            get { return _property.Name; }
        }

        #endregion

        private void IsRequired(PropertyInfo property)
        {
            foreach (object attribute in property.GetCustomAttributes(false))
            {
                _required |= attribute is RequiredAttribute;
            }
        }

        private void DetermineDefault(PropertyInfo property)
        {
            var fallBack = (IDefaultPropertyValue)property.GetCustomAttributes(typeof(IDefaultPropertyValue), false).FirstOrDefault(); 
            if(fallBack!=null)
            {
                _default = fallBack.ToString();
            }
            ;
        }

        public Array EnumValues
        {
            get
            {
                Array enumValues = null;
                foreach (object attribute in _property.GetCustomAttributes(false))
                {
                    if(attribute is EnumProperyTypeAttribute)
                    {
                        enumValues = (attribute as EnumProperyTypeAttribute).EnumValues;
                    }
                }
                return enumValues;
            }
        }
    }
}
