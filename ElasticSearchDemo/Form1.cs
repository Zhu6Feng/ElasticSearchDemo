using Nest;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ElasticSearchDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void showForm(List<WebModel> ListME)
        {
            Form2 fm2 = new Form2(ListME);
            fm2.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<WebModel> ListME = ESSearch.GetResult_RegexpQuery(textBox2.Text, textBox1.Text, false);
            if (ListME.Count>0)
            {
                showForm(ListME);
            }
            else
            {
                MessageBox.Show("暂无结果~");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<WebModel> ListME = ESSearch.GetResult_TermQuery(textBox2.Text, textBox1.Text, false);
            if (ListME.Count > 0)
            {
                showForm(ListME);
            }
            else
            {
                MessageBox.Show("暂无结果~");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ESSearch search = new ESSearch();
            List<WebModel> ListME = ESSearch.GetResult_MatchQuery(textBox2.Text, textBox1.Text, false);
            if (ListME.Count > 0)
            {
                showForm(ListME);
            }
            else
            {
                MessageBox.Show("暂无结果~");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox4.Text) && !string.IsNullOrEmpty(textBox5.Text) && !string.IsNullOrEmpty(textBox6.Text))
            {
                List<WebModel> ListME = new List<WebModel>();
                WebModel me = new WebModel();
                Random rd = new Random();
                bool ifSuccess = false;
                me.id = rd.Next(0, 9999).ToString();
                me.title = this.textBox4.Text;
                me.sortid = rd.Next(0, 9999);
                me.url = this.textBox5.Text;
                me.content = this.textBox6.Text;
                ListME.Add(me);
                ifSuccess = ESProvider.BulkPopulateIndex(ListME, textBox3.Text);
                if (ifSuccess)
                {
                    MessageBox.Show("更新成功！");
                }
                else
                {
                    MessageBox.Show("更新失败！");
                }
            }
            else
            {
                MessageBox.Show("请填写必要信息！");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox5.Text))
            {
                bool ifSuccess = ESProvider.DeleteIndex(textBox3.Text, textBox5.Text);
                if (ifSuccess)
                {
                    MessageBox.Show("删除成功！");
                }
                else
                {
                    MessageBox.Show("删除失败！");
                }
            }
            else
            {
                MessageBox.Show("请填写索引、类型、URL(用作ID)！");
            }
        }
    }
}
