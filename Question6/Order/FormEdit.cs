using Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace appForm
{
    public partial class FormEdit : Form
    {
        public app.Order CurrentOrder { get; set; }
        public FormEdit()
        {
            InitializeComponent();
            //customerBindingSource.Add(new Customer("1", "li"));
            //customerBindingSource.Add(new Customer("2", "zhang"));

        }
        public FormEdit(app.Order order, bool editMode = false) : this()
        {
            //TODO 如果想实现不点保存只关窗口后订单不变化，需要把order深克隆给CurrentOrder
            CurrentOrder = order;
            orderBindingSource.DataSource = CurrentOrder;
            textBox1.Enabled = !editMode;
            if (!editMode)
            {
                //CurrentOrder.Customer = (Customer)customerBindingSource.Current;
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            FormItemsEdit formItemEdit = new FormItemsEdit(new app.OrderDetails());
            //FormItemsEdit formItemEdit = new FormItemEdit(new OrderDetail());
            try
            {
                if (formItemEdit.ShowDialog() == DialogResult.OK)
                {
                    CurrentOrder.AddItem(formItemEdit.OrderItem);
                    itemsBindingSource.ResetBindings(false);
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CurrentOrder.Details.ForEach(item =>
            {
                item.OrderId = CurrentOrder.Id;
            });
            this.Close();
        }

        private void EditItem()
        {
            app.OrderDetails orderItem = itemsBindingSource.Current as app.OrderDetails;
            if (orderItem == null)
            {
                MessageBox.Show("请选择一个订单项进行修改");
                return;
            }
            FormItemsEdit formItemEdit = new FormItemsEdit(orderItem);
            if (formItemEdit.ShowDialog() == DialogResult.OK)
            {
                itemsBindingSource.ResetBindings(false);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            app.OrderDetails orderItem = itemsBindingSource.Current as app.OrderDetails;
            if (orderItem == null)
            {
                MessageBox.Show("请选择一个订单项进行删除");
                return;
            }
            CurrentOrder.RemoveItem(orderItem);
            itemsBindingSource.ResetBindings(false);
        }
    }
}
