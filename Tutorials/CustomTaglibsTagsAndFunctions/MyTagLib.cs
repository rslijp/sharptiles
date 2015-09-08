using System;
using System.Collections;
using System.Collections.Generic;
using org.SharpTiles.Common;
using org.SharpTiles.Tags;
using org.SharpTiles.Tags.CoreTags;

namespace ActionsAndUrls
{
    public class MyTagLib : BaseTagGroup<MyTagLib>
    {
        static MyTagLib()
        {
            Register<Now>();
            Register<Ping>();
            Register<Simple>();
            Register<Session>();
            Register<Any>();
            Register<NiceBody>();
            Register<Jar>();
            Register<Pickle>();
        }

        public override string Name
        {
            get { return "my"; }
        }
    }

    internal class Now : BaseCoreTag, ITag
    {
        public static readonly string NAME = "now";

        public string TagName
        {
            get { return NAME; }
        }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public string Evaluate(TagModel model)
        {
            return DateTime.Now.ToString();
        }
    }

    internal class Simple : BaseCoreTag, ITag
    {
        public static readonly string NAME = "simple";

        public string TagName
        {
            get { return NAME; }
        }

        public ITagAttribute Message { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public string Evaluate(TagModel model)
        {
            return Message != null ? Message.Evaluate(model).ToString() : "-";
        }
    }

    internal class Ping : BaseCoreTag, ITag
    {
        public static readonly string NAME = "ping";

        public string TagName
        {
            get { return NAME; }
        }

        [TagDefaultValue("SORRY")]
        public ITagAttribute Message { get; set; }

        [TagDefaultProperty("Message")]
        public ITagAttribute Override { get; set; }


        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public string Evaluate(TagModel model)
        {
            return GetAutoValueAsString("Override", model);
        }
    }

    internal class NiceBody : BaseCoreTag, ITag
    {
        public static readonly string NAME = "niceBody";

        public string TagName
        {
            get { return NAME; }
        }

        [TagDefaultValue("EMPTY")]
        public ITagAttribute Body { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        public string Evaluate(TagModel model)
        {
            var body = GetAutoValueAsString("Body", model);
            return body.Length + ":" + body;
        }
    }


    internal class Session : BaseCoreTag, ITag
    {
        public static readonly string NAME = "session";

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Name { get; set; }


        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public string Evaluate(TagModel model)
        {
            var obj = model.Session[GetAutoValueAsString("Name", model)];
            return obj != null ? obj.ToString() : String.Empty;
        }
    }

    internal class Any : BaseCoreTag, ITag
    {
        public static readonly string NAME = "any";

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Name { get; set; }


        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.None; }
        }

        public string Evaluate(TagModel model)
        {
            var obj = model[GetAutoValueAsString("Name", model)];
            return obj != null ? obj.ToString() : String.Empty;
        }
    }

    internal class Jar : BaseCoreTag, ITagWithNestedTags
    {
        public static readonly string NAME = "jar";
        private readonly List<Pickle> _nestedTags = new List<Pickle>();

        public string TagName
        {
            get { return NAME; }
        }

        [Required]
        public ITagAttribute Index { get; set; }

        

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.NestedTags; }
        }

        public string Evaluate(TagModel model)
        {
            return GetAsString(_nestedTags[GetAutoValueAsInt("Index", model).Value-1].Body, model);
        }
        

        public void AddNestedTag(ITag tag)
        {
            if (tag is Pickle)
            {
                _nestedTags.Add(tag as Pickle);
            }
            else
            {
                throw TagException.OnlyNestedTagsOfTypeAllowed(tag.GetType(), typeof(Jar)).Decorate(tag.Context);
            }
        }
    }

    internal class Pickle : BaseCoreTag, ITag
    {
        public static readonly string NAME = "pickle";

        public string TagName
        {
            get { return NAME; }
        }

        public ITagAttribute Body { get; set; }

        public TagBodyMode TagBodyMode
        {
            get { return TagBodyMode.Free; }
        }

        public string Evaluate(TagModel model)
        {
            throw TagException.NotAllowedShouldBePartOf(GetType(), typeof(Jar)).Decorate(Context);
        }
    }

}