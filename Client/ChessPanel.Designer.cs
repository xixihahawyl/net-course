namespace Client
{
    partial class ChessPanel
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ChessPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.DarkOrange;
            this.BackgroundImage = global::Client.Resource.back;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "ChessPanel";
            this.Size = new System.Drawing.Size(604, 464);
            this.SizeChanged += new System.EventHandler(this.ChessPanel_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ChessPanel_Paint);
            this.DoubleClick += new System.EventHandler(this.ChessPanel_DoubleClick);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChessPanel_MouseClick);
            this.Resize += new System.EventHandler(this.ChessPanel_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
