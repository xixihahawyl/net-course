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
namespace Order
{
    public partial class FormItemsEdit : Form
    {
        public OrderDetails OrderItem { get; set; }
        public FormItemsEdit()
        {
            InitializeComponent();
        }
        public FormItemsEdit(OrderDetails orderItem) : this()
        {
            this.OrderItem = orderItem;
            this.itemsBindingSource.DataSource = orderItem;
        }

    }
}
