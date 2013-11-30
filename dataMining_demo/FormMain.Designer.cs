using System.Windows.Forms;

namespace dataMining_demo
{
    partial class FormMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.задачаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.кластеризацииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.прогнозированияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.представлениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выборкуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.структуруToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.модельToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.вариантАлгоритмаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.результатыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сервисToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаСоединенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.схемаОбъектовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(21, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 37);
            this.button1.TabIndex = 1;
            this.button1.Text = "Обработать модель";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Выбор представления:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 40);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Выборка данных:";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(7, 84);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 5;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Модель ИАД:";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(7, 127);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 5;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox4);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 197);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Выбор объектов ИАД";
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(9, 170);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(121, 21);
            this.comboBox4.TabIndex = 5;
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Структура ИАД:";
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button6.Location = new System.Drawing.Point(6, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(95, 39);
            this.button6.TabIndex = 10;
            this.button6.Text = "Метаданные";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button7.Location = new System.Drawing.Point(10, 78);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(93, 39);
            this.button7.TabIndex = 11;
            this.button7.Text = "Детализация";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Location = new System.Drawing.Point(12, 283);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(128, 123);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Результат";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.задачаToolStripMenuItem,
            this.создатьToolStripMenuItem,
            this.результатыToolStripMenuItem,
            this.сервисToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(302, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // задачаToolStripMenuItem
            // 
            this.задачаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.кластеризацииToolStripMenuItem,
            this.прогнозированияToolStripMenuItem});
            this.задачаToolStripMenuItem.Name = "задачаToolStripMenuItem";
            this.задачаToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.задачаToolStripMenuItem.Text = "Задача";
            // 
            // кластеризацииToolStripMenuItem
            // 
            this.кластеризацииToolStripMenuItem.Name = "кластеризацииToolStripMenuItem";
            this.кластеризацииToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.кластеризацииToolStripMenuItem.Text = "Кластеризации";
            this.кластеризацииToolStripMenuItem.Click += new System.EventHandler(this.кластеризацииToolStripMenuItem_Click);
            // 
            // прогнозированияToolStripMenuItem
            // 
            this.прогнозированияToolStripMenuItem.Name = "прогнозированияToolStripMenuItem";
            this.прогнозированияToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.прогнозированияToolStripMenuItem.Text = "Прогнозирования";
            this.прогнозированияToolStripMenuItem.Click += new System.EventHandler(this.прогнозированияToolStripMenuItem_Click);
            // 
            // создатьToolStripMenuItem
            // 
            this.создатьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.представлениеToolStripMenuItem,
            this.выборкуToolStripMenuItem,
            this.структуруToolStripMenuItem1,
            this.модельToolStripMenuItem,
            this.вариантАлгоритмаToolStripMenuItem});
            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.создатьToolStripMenuItem.Text = "Создать";
            // 
            // представлениеToolStripMenuItem
            // 
            this.представлениеToolStripMenuItem.Name = "представлениеToolStripMenuItem";
            this.представлениеToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.представлениеToolStripMenuItem.Text = "Представление";
            this.представлениеToolStripMenuItem.Click += new System.EventHandler(this.представлениеToolStripMenuItem_Click);
            // 
            // выборкуToolStripMenuItem
            // 
            this.выборкуToolStripMenuItem.Name = "выборкуToolStripMenuItem";
            this.выборкуToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.выборкуToolStripMenuItem.Text = "Выборку данных";
            this.выборкуToolStripMenuItem.Click += new System.EventHandler(this.выборкуToolStripMenuItem_Click);
            // 
            // структуруToolStripMenuItem1
            // 
            this.структуруToolStripMenuItem1.Name = "структуруToolStripMenuItem1";
            this.структуруToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.структуруToolStripMenuItem1.Text = "Структуру DM";
            this.структуруToolStripMenuItem1.Click += new System.EventHandler(this.структуруToolStripMenuItem1_Click);
            // 
            // модельToolStripMenuItem
            // 
            this.модельToolStripMenuItem.Name = "модельToolStripMenuItem";
            this.модельToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.модельToolStripMenuItem.Text = "Модель DM";
            this.модельToolStripMenuItem.Click += new System.EventHandler(this.модельToolStripMenuItem_Click);
            // 
            // вариантАлгоритмаToolStripMenuItem
            // 
            this.вариантАлгоритмаToolStripMenuItem.Name = "вариантАлгоритмаToolStripMenuItem";
            this.вариантАлгоритмаToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.вариантАлгоритмаToolStripMenuItem.Text = "Вариант алгоритма";
            this.вариантАлгоритмаToolStripMenuItem.Click += new System.EventHandler(this.вариантАлгоритмаToolStripMenuItem_Click);
            // 
            // результатыToolStripMenuItem
            // 
            this.результатыToolStripMenuItem.Name = "результатыToolStripMenuItem";
            this.результатыToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.результатыToolStripMenuItem.Text = "Результаты";
            this.результатыToolStripMenuItem.Click += new System.EventHandler(this.результатыToolStripMenuItem_Click);
            // 
            // сервисToolStripMenuItem
            // 
            this.сервисToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкаСоединенияToolStripMenuItem,
            this.схемаОбъектовToolStripMenuItem});
            this.сервисToolStripMenuItem.Name = "сервисToolStripMenuItem";
            this.сервисToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.сервисToolStripMenuItem.Text = "Сервис";
            // 
            // настройкаСоединенияToolStripMenuItem
            // 
            this.настройкаСоединенияToolStripMenuItem.Name = "настройкаСоединенияToolStripMenuItem";
            this.настройкаСоединенияToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.настройкаСоединенияToolStripMenuItem.Text = "Настройка соединения";
            // 
            // схемаОбъектовToolStripMenuItem
            // 
            this.схемаОбъектовToolStripMenuItem.Name = "схемаОбъектовToolStripMenuItem";
            this.схемаОбъектовToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.схемаОбъектовToolStripMenuItem.Text = "Схема объектов";
            this.схемаОбъектовToolStripMenuItem.Click += new System.EventHandler(this.схемаОбъектовToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 413);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "dataMining";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox3;
        private ComboBox comboBox4;
        private Label label4;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem задачаToolStripMenuItem;
        private ToolStripMenuItem кластеризацииToolStripMenuItem;
        private ToolStripMenuItem прогнозированияToolStripMenuItem;
        private ToolStripMenuItem создатьToolStripMenuItem;
        private ToolStripMenuItem представлениеToolStripMenuItem;
        private ToolStripMenuItem выборкуToolStripMenuItem;
        private ToolStripMenuItem структуруToolStripMenuItem1;
        private ToolStripMenuItem модельToolStripMenuItem;
        private ToolStripMenuItem вариантАлгоритмаToolStripMenuItem;
        private ToolStripMenuItem результатыToolStripMenuItem;
        private ToolStripMenuItem сервисToolStripMenuItem;
        private ToolStripMenuItem настройкаСоединенияToolStripMenuItem;
        private ToolStripMenuItem схемаОбъектовToolStripMenuItem;


    }
}

