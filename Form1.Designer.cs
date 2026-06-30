using System;
using System.Windows.Forms;

namespace Practika
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox comboBoxCountry;
        private DateTimePicker dateTimePickerYear;
        private Button buttonGetHolidays;
        private ListBox listBoxHolidays;
        private Label label1;
        private Label label2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.comboBoxCountry = new ComboBox();
            this.dateTimePickerYear = new DateTimePicker();
            this.buttonGetHolidays = new Button();
            this.listBoxHolidays = new ListBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.SuspendLayout();

            // comboBoxCountry
            this.comboBoxCountry.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCountry.Location = new System.Drawing.Point(120, 20);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new System.Drawing.Size(200, 21);
            this.comboBoxCountry.TabIndex = 0;

            // dateTimePickerYear
            this.dateTimePickerYear.Format = DateTimePickerFormat.Custom;
            this.dateTimePickerYear.CustomFormat = "yyyy";
            this.dateTimePickerYear.ShowUpDown = true;
            this.dateTimePickerYear.Location = new System.Drawing.Point(120, 60);
            this.dateTimePickerYear.Name = "dateTimePickerYear";
            this.dateTimePickerYear.Size = new System.Drawing.Size(80, 20);
            this.dateTimePickerYear.TabIndex = 1;
            this.dateTimePickerYear.Value = new DateTime(DateTime.Now.Year, 1, 1);

            // buttonGetHolidays
            this.buttonGetHolidays.Location = new System.Drawing.Point(220, 55);
            this.buttonGetHolidays.Name = "buttonGetHolidays";
            this.buttonGetHolidays.Size = new System.Drawing.Size(100, 30);
            this.buttonGetHolidays.TabIndex = 2;
            this.buttonGetHolidays.Text = "Получить";
            this.buttonGetHolidays.UseVisualStyleBackColor = true;
            this.buttonGetHolidays.Click += new System.EventHandler(this.buttonGetHolidays_Click);

            // listBoxHolidays
            this.listBoxHolidays.FormattingEnabled = true;
            this.listBoxHolidays.Location = new System.Drawing.Point(20, 100);
            this.listBoxHolidays.Name = "listBoxHolidays";
            this.listBoxHolidays.Size = new System.Drawing.Size(300, 250);
            this.listBoxHolidays.TabIndex = 3;

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Страна:";

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Год:";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 380);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxHolidays);
            this.Controls.Add(this.buttonGetHolidays);
            this.Controls.Add(this.dateTimePickerYear);
            this.Controls.Add(this.comboBoxCountry);
            this.Name = "Form1";
            this.Text = "Календарь праздников";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}