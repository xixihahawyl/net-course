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
        OrderService orderService;
        BindingSource fieldsBS = new BindingSource();
        public String Keyword { get; set; }
        public Form1()
        {
            InitializeComponent();
            orderService = new OrderService();
            OrderDetails detail_1 = new OrderDetails(2, 2, "apple");
            OrderDetails detail_2 = new OrderDetails(3, 3, "pear");
            List<OrderDetails> dets = new List<OrderDetails>();
            dets.Add(detail_1);dets.Add(detail_2);
            app.Order order1 = new app.Order(1,"王五",dets);
            detail_1 = new OrderDetails(3, 2, "banana");
            detail_2 = new OrderDetails(4, 1, "orange");
            dets.Clear();
            dets.Add(detail_1); dets.Add(detail_2);
            app.Order order2 = new app.Order(2, "李四", dets);
            orderService.addOrder(order1);
            orderService.addOrder(order2);
            orderBindingSource.DataSource = orderService.Orders;
            //comboBox1.SelectedIndex = 0;
            textBox1.DataBindings.Add("text",this,"keyword");//数据绑定
            

        }


        private void add_Click(object sender, EventArgs e)
        {
            FormEdit form2 = new FormEdit(new app.Order());
            if (form2.ShowDialog() == DialogResult.OK)
            {
                orderService.addOrder(form2.CurrentOrder);
                QueryAll();
            }
        }
        private void QueryAll()
        {
            orderBindingSource.DataSource = orderService.Orders;
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
                orderService.UpdateOrder(form2.CurrentOrder);
                QueryAll();
            }
        }
    }
}
