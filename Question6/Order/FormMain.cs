using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using app;

namespace appForm
{
    public partial class Form1 : Form
    {
        BindingSource fieldsBS = new BindingSource();
        public String Keyword { get; set; }
        public Form1()
        {
            InitializeComponent();
            // orderService = new OrderService();
            //OrderDetails detail_1 = new OrderDetails(2, 2, "apple");
            // OrderDetails detail_2 = new OrderDetails(3, 3, "pear");
            //List<OrderDetails> dets = new List<OrderDetails>();
            // dets.Add(detail_1);dets.Add(detail_2);
            //app.Order order1 = new app.Order(1,"王五",dets);
            //  detail_1 = new OrderDetails(3, 2, "banana");
            // detail_2 = new OrderDetails(4, 1, "orange");
            // dets.Clear();
            //dets.Add(detail_1); dets.Add(detail_2);
            // app.Order order2 = new app.Order(2, "李四", dets);
            // orderService.addOrder(order1);
            // orderService.addOrder(order2);
            // orderBindingSource.DataSource = orderService.Orders;
            comboBox1.SelectedIndex = 0;
            textBox1.DataBindings.Add("text", this, "keyword");//数据绑定
            orderBindingSource.DataSource = app.OrderService.GetAllOrders();


        }


        private void add_Click(object sender, EventArgs e)
        {
            FormEdit form2 = new FormEdit(new app.Order());
            if (form2.ShowDialog() == DialogResult.OK)
            {
                app.OrderService.AddOrder(form2.CurrentOrder);
                QueryAll();
            }
        }
        private void QueryAll()
        {
            orderBindingSource.DataSource = app.OrderService.GetAllOrders();
            orderBindingSource.ResetBindings(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditOrder();
        }
        private void EditOrder()
        {
            app.Order order = orderBindingSource.Current as app.Order;
            if (order == null)
            {
                MessageBox.Show("请选择一个订单进行修改");
                return;
            }
            FormEdit form2 = new FormEdit(order, true);
            if (form2.ShowDialog() == DialogResult.OK)
            {
                app.OrderService.UpdateOrder(form2.CurrentOrder);
                QueryAll();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0://所有订单
                    orderBindingSource.DataSource = app.OrderService.GetAllOrders();
                    break;
                case 1://根据ID查询
                    app.Order order = app.OrderService.GetOrder(Keyword);
                    List<app.Order> result = new List<app.Order>();
                    if (order != null) result.Add(order);
                    orderBindingSource.DataSource = result;
                    break;
            }
            orderBindingSource.ResetBindings(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result.Equals(DialogResult.OK))
            {
                String fileName = saveFileDialog1.FileName;
                app.OrderService.Export(fileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result.Equals(DialogResult.OK))
            {
                String fileName = openFileDialog1.FileName;
                app.OrderService.Import(fileName);
                QueryAll();
            }
        }
    }
}
