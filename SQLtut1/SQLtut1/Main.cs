using System;
using System.Windows.Forms;

namespace SQLtut1
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void businessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BizContacts frm = new BizContacts();
            frm.MdiParent = this;
            frm.Show();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }
    }
}
