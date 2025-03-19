using DevExpress.Xpf.Accordion;
using Sedna.Service.Requirements.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Senda.Requirements.Capture.UI
{
    public class AccordionSelector : IChildrenSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item is Topic)
            {
                return ((Topic)item).TopicSearches;
            }
            else
            {
                return null;
            }
        }
    }
}
