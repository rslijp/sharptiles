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
}