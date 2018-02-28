using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElasticSearchDemo
{
    public partial class Form2 : Form
    {
        public Form2(List<WebModel> ListME)
        {
            InitializeComponent();
            dataGridView1.DataSource = ListME;
        }
    }
}
