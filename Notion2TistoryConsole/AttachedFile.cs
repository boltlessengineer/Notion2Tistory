using System;
using System.Collections.Generic;
using System.Text;

namespace Notion2TistoryConsole
{
    class AttachedFile
    {
        public string originalTag;
        public string originalPath;

        public string replacer = "[##_abcd|||_##]";
    }

    class AttachedImage : AttachedFile
    {
        public string originalStyle;
        public string originalCaption;
    }
}
