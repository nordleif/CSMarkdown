using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DTable
{
    class Rows : ReadOnlyCollection<Row>
    {
        public Rows(IList<Row> list) : base(list)
        {

        }

        new public Cell[] this[int index]
        {
            get
            {

                return null;
            }
        }
    }
}
