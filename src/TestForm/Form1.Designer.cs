namespace TestForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.btnEnqueue = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEnqueueSalesState = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSendWzdEventNew = new System.Windows.Forms.Button();
            this.btnSendWzdEvent = new System.Windows.Forms.Button();
            this.btnSendString = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Location = new System.Drawing.Point(12, 20);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(75, 23);
            this.btnSubscribe.TabIndex = 0;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // btnEnqueue
            // 
            this.btnEnqueue.Location = new System.Drawing.Point(102, 20);
            this.btnEnqueue.Name = "btnEnqueue";
            this.btnEnqueue.Size = new System.Drawing.Size(149, 23);
            this.btnEnqueue.TabIndex = 1;
            this.btnEnqueue.Text = "Enqueue(PriceChanged)";
            this.btnEnqueue.UseVisualStyleBackColor = true;
            this.btnEnqueue.Click += new System.EventHandler(this.btnEnqueue_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnEnqueueSalesState);
            this.groupBox1.Controls.Add(this.btnEnqueue);
            this.groupBox1.Controls.Add(this.btnSubscribe);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(555, 67);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BizSocketEventNet";
            // 
            // btnEnqueueSalesState
            // 
            this.btnEnqueueSalesState.Location = new System.Drawing.Point(257, 20);
            this.btnEnqueueSalesState.Name = "btnEnqueueSalesState";
            this.btnEnqueueSalesState.Size = new System.Drawing.Size(185, 23);
            this.btnEnqueueSalesState.TabIndex = 2;
            this.btnEnqueueSalesState.Text = "Enqueue(PublishSalesState)";
            this.btnEnqueueSalesState.UseVisualStyleBackColor = true;
            this.btnEnqueueSalesState.Click += new System.EventHandler(this.btnEnqueueSalesState_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSendString);
            this.groupBox2.Controls.Add(this.btnSendWzdEventNew);
            this.groupBox2.Controls.Add(this.btnSendWzdEvent);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(555, 69);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // btnSendWzdEventNew
            // 
            this.btnSendWzdEventNew.Location = new System.Drawing.Point(170, 29);
            this.btnSendWzdEventNew.Name = "btnSendWzdEventNew";
            this.btnSendWzdEventNew.Size = new System.Drawing.Size(137, 23);
            this.btnSendWzdEventNew.TabIndex = 1;
            this.btnSendWzdEventNew.Text = "SendWzdEventNew";
            this.btnSendWzdEventNew.UseVisualStyleBackColor = true;
            this.btnSendWzdEventNew.Click += new System.EventHandler(this.btnSendWzdEventNew_Click);
            // 
            // btnSendWzdEvent
            // 
            this.btnSendWzdEvent.Location = new System.Drawing.Point(18, 29);
            this.btnSendWzdEvent.Name = "btnSendWzdEvent";
            this.btnSendWzdEvent.Size = new System.Drawing.Size(137, 23);
            this.btnSendWzdEvent.TabIndex = 0;
            this.btnSendWzdEvent.Text = "SendWzdEvent";
            this.btnSendWzdEvent.UseVisualStyleBackColor = true;
            this.btnSendWzdEvent.Click += new System.EventHandler(this.btnSendWzdEvent_Click);
            // 
            // btnSendString
            // 
            this.btnSendString.Location = new System.Drawing.Point(325, 29);
            this.btnSendString.Name = "btnSendString";
            this.btnSendString.Size = new System.Drawing.Size(137, 23);
            this.btnSendString.TabIndex = 2;
            this.btnSendString.Text = "SendString";
            this.btnSendString.UseVisualStyleBackColor = true;
            this.btnSendString.Click += new System.EventHandler(this.btnSendString_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 136);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.Button btnEnqueue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnEnqueueSalesState;
        private System.Windows.Forms.Button btnSendWzdEvent;
        private System.Windows.Forms.Button btnSendWzdEventNew;
        private System.Windows.Forms.Button btnSendString;
    }
}

