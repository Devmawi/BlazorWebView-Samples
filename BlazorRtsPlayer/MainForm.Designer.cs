namespace BlazorRtsPlayer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainBlazorWebView = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            this.SuspendLayout();
            // 
            // mainBlazorWebView
            // 
            this.mainBlazorWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainBlazorWebView.Location = new System.Drawing.Point(0, 0);
            this.mainBlazorWebView.Name = "mainBlazorWebView";
            this.mainBlazorWebView.Size = new System.Drawing.Size(800, 450);
            this.mainBlazorWebView.TabIndex = 0;
            this.mainBlazorWebView.Text = "blazorWebView1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainBlazorWebView);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView mainBlazorWebView;
    }
}