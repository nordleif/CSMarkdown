using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CSMarkdown.DataTable
{
    class ColumnArray : Collection<Column>
    {
        new public Cell[] this[int index]
        {
            get
            {
                return null;
            }
        }
    }
}
