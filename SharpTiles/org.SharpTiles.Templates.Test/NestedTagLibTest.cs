using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace org.SharpTiles.Templates.Test
{
    [TestFixture]
    public class NestedTagLibTest
    {
        public class OutputTagLib : BaseTagGroup<Core>
        {
            public OutputTagLib()
            {
                Register<OutputTag>();
                Register<Output2Tag>();

            }

            public override string Name => "nested";
        }

        public class OutputTag : BaseCoreTag, ITagWithNestedTags, ITagExtendTagLib
        {
            public string TagName => "output";
            private IList<ITag> _list = new List<ITag>();

            public TagBodyMode TagBodyMode => TagBodyMode.NestedTags;

            public string Evaluate(TagModel model)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetType().Name);
                foreach (var tag in _list)
                {
                    sb.Append(".");
                    sb.Append(tag.Evaluate(model));

                }
                return sb.ToString();
            }

            public void AddNestedTag(ITag tag)
            {
                _list.Add(tag);
            }

            public ITag[] NestedTags => _list.ToArray();
            public ITagGroup TagLibExtension => new NestedTagGroup(Group.Name, typeof(OutputTag));
        }

        public class Output2Tag : BaseCoreTag, ITagWithNestedTags, ITagExtendTagLib
        {
            public string TagName => "output2";
            private IList<ITag> _list = new List<ITag>();

            public TagBodyMode TagBodyMode => TagBodyMode.NestedTags;

            public string Evaluate(TagModel model)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetType().Name);
                foreach (var tag in _list)
                {
                    sb.Append(".");
                    sb.Append(tag.Evaluate(model));

                }
                return sb.ToString();
            }

            public void AddNestedTag(ITag tag)
            {
                _list.Add(tag);
            }

            public ITag[] NestedTags => _list.ToArray();
            public ITagGroup TagLibExtension => new NestedTagGroup(Group.Name, typeof(Output2NestedTag));
        }

        public class Output2NestedTag : Output2Tag
        {
        }

        public class TestModel
        {
            public string Text { get; set; }
            public TestModel Nested { get; set; }
        }

        [Test]
        public void No_Nesting()
        {
            const string TEMPLATE = "<output/>";
            const string FORMATTED = "OutputTag";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).OverrideLib(new TagLib(TagLibMode.StrictResolve, new OutputTagLib())).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void Same_Nesting()
        {
            const string TEMPLATE = "<output><output/></output>";
            const string FORMATTED = "OutputTag.OutputTag";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).OverrideLib(new TagLib(TagLibMode.StrictResolve, new OutputTagLib())).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void Different_Nesting()
        {
            const string TEMPLATE = "<output2><output2/></output2>";
            const string FORMATTED = "Output2Tag.Output2NestedTag";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).OverrideLib(new TagLib(TagLibMode.StrictResolve, new OutputTagLib())).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }

        [Test]
        public void More_Different_Nesting()
        {
            const string TEMPLATE = "<output2><output2><output2/></output2></output2>";
            const string FORMATTED = "Output2Tag.Output2NestedTag.Output2NestedTag";
            var model = new TestModel();
            model.Text = "";
            var formatter = new Formatter(TEMPLATE).OverrideLib(new TagLib(TagLibMode.StrictResolve, new OutputTagLib())).Parse();
            string formatted = formatter.Format(model);
            Assert.That(formatted, Is.EqualTo(FORMATTED));
        }
    }
}
