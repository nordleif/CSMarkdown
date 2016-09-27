using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMarkdown.DataTable
{
    class Test
    {
        void test()
        {
            var dt = new DTable();

            dt.Columns[0].OrderBy(c => c.Value).Last().BackgroundColor = "Blue";
        }
    }
}
