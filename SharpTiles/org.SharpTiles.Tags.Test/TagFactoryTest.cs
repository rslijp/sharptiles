using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using org.SharpTiles.Tags.CoreTags;
using org.SharpTiles.Templates;

namespace org.SharpTiles.Tags
{
    [TestFixture]
    public class TagFactoryTest
    {
        public class ColorTagFactory : ITagFactory
        {
            private readonly string _color;

            public ColorTagFactory(string color)
            {
                _color = color;
            }

            public ITag NewInstance()
            {
                return new ColorTag(_color);
            }

            public string Name => NewInstance().TagName;
        }

        public class ColorTag : BaseCoreTag, ITag
        {
            private string _color;

            public ColorTag(string color)
            {
                _color = color;
            }

            [Required]
            private ITagAttribute Body { get; set; }

            public string TagName => $"color-{_color}";
            public TagBodyMode TagBodyMode => TagBodyMode.Free;
            public string Evaluate(TagModel model)
            {
                var body = GetAutoValueAsString(nameof(Body), model);
                return $"<div style='color: ${_color}>{body}</div>";
            }
        }

        public class Colors : BaseTagGroup<Colors>
        {
            public Colors(params string[] colors)
            {
                foreach (var color in colors)
                {
                    Register(new ColorTagFactory(color));
                }                
            }
            public override string Name => "colors";
        }

        [Test]
        public void Register()
        {
            new Formatter().OverrideLib(new TagLib(TagLibMode.StrictResolve, new Colors("red", "green", "purple")));
        }
    }
}
