using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.SharpTiles.Common
{
    public static class IModelExtensions
    {
        public static IModel AdeptToNaturalLanguage(this IModel model)
        {
            return new NaturalLanguageModelWrapper(model);
        }

        public class NaturalLanguageModelWrapper : IModel
        {
            private IModel inner;

            public NaturalLanguageModelWrapper(IModel reflection)
            {
                this.inner = reflection;
            }

            public object TryGet(string property)
            {
                return inner.TryGet(LanguageHelper.CamelCaseProperty(property));
            }

            public ReflectionResult Get(string property)
            {
                return inner.Get(LanguageHelper.CamelCaseProperty(property));
            }

            public object this[string property]
            {
                get { return inner[LanguageHelper.CamelCaseProperty(property)]; }
                set { inner[LanguageHelper.CamelCaseProperty(property)] = value; }
            }
        }
    }
}
