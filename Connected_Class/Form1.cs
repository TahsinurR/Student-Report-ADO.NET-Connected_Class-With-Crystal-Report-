using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Configuration;

namespace Connected_Class
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MemoryStream ms;
        DataTable dt = new DataTable();//disconnected class
        private void Form1_Load(object sender, EventArgs e)
        {
            InitialWorks1();
            InitialWorks2();

            //Foreign Key table columns, in our project student is primary key table, fees is foregn key table
            dataGridView1.Columns.Add("BATCH", "BATCH");
            dataGridView1.Columns.Add("SLNO", "SLNO");
            dataGridView1.Columns.Add("ADDRESS", "ADDRESS");
            dataGridView1.Columns.Add("FEE", "FEE");
            dataGridView1.Columns.Add("DATE", "DATE");

        }
        private void InitialWorks1()
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["master"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                try
                {
                    using (SqlCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = $"if not exists(select * from sysdatabases where name='OLI') create database OLI;";
                        cmd.ExecuteNonQuery();




                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void InitialWorks2()
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"if not exists(select * from sysobjects where name='Student')create table Student(ID varchar(100) primary key, NAME varchar(100), PHOTO image, STRINGPHOTO varchar(200))";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"if not exists(select * from sysobjects where name='Fees')create table Fees(BATCH varchar(100), SLNO int, ADDRESS varchar(100), FEE money, DATE datetime, STUDENTID varchar(100) references student(ID), primary key(BATCH, SLNO))";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            dataGridView1.Rows.Clear();

                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                    textBox7.Text = openFileDialog1.FileName;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            BlankTextBoxes();
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            ConnectionDB a = new ConnectionDB();
            a.conn1("select distinct Id from student");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())//until last data
            {
                comboBox1.Items.Add(rdr[0].ToString());//0=> first field, Id
            }
        }
        byte[] conv_photo()
        {
            byte[] photo_aray = { };
            //converting photo to binary data
            if (pictureBox1.Image != null)
            {
                ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);
            }
            return photo_aray;
        }

        private void BlankTextBoxes()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            textBox1.Focus();

        }

       

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                DataRow dr = dt.NewRow();

                dr[0] = textBox3.Text;
                dr[1] = textBox4.Text;
                dr[2] = textBox5.Text;
                dr[3] = textBox6.Text;
                dr[4] = dateTimePicker1.Value;
                dt.Rows.Add(dr);
                dataGridView1.DataSource = dt;
                return;
            }

            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dataGridView1);
            newRow.Cells[0].Value = textBox3.Text;
            newRow.Cells[1].Value = textBox4.Text;
            newRow.Cells[2].Value = textBox5.Text;
            newRow.Cells[3].Value = textBox6.Text;
            newRow.Cells[4].Value = dateTimePicker1.Value;
            dataGridView1.Rows.Add(newRow);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "jpeg|*.jpg|bmp|*.bmp|all files|*.*";
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    pictureBox2.ImageLocation = openFileDialog1.FileName;
                    textBox8.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"delete from fees where studentid='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from student where id='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();



                            string fn = Path.GetFileName(textBox8.Text);
                            string path = AppDomain.CurrentDomain.BaseDirectory + @"Images\" + fn.ToString();
                            if (!File.Exists(path))
                            {
                                File.Copy(textBox8.Text, path);
                            }
                            string dt = dateTimePicker1.Value.ToShortDateString();

                            cmd.CommandText = $"insert into student values(@id, @name, @pic, @picstring)";
                            cmd.Parameters.Add(new SqlParameter("@id", textBox1.Text));
                            cmd.Parameters.Add(new SqlParameter("@name", textBox2.Text));
                            //cmd.Parameters.Add(new SqlParameter("@fee", double.Parse(textBox6.Text)));
                            //cmd.Parameters.Add(new SqlParameter("@dt", dt));
                            cmd.Parameters.Add(new SqlParameter("@pic", conv_photo()));
                            cmd.Parameters.Add(new SqlParameter("@picstring", "Images\\" + fn.ToString()));
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                            {
                            //MessageBox.Show($"insert into fees values('{dataGridView1.Rows[i].Cells[0].Value.ToString()}', {int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString())}, '{dataGridView1.Rows[i].Cells[2].Value.ToString()}', '{int.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString())}', '{textBox1.Text}')");
                           // string s = $"'{int.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString())}',";
                            //MessageBox.Show(s);
                            cmd.CommandText = $"insert into fees values('{dataGridView1.Rows[i].Cells[0].Value.ToString()}', {int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString())}, '{dataGridView1.Rows[i].Cells[2].Value.ToString()}', '{double.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString())}','{(dataGridView1.Rows[i].Cells[4].Value.ToString())}','{textBox1.Text}')";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();


                        }
                            tran.Commit();

                        }
                        dataGridView1.DataSource = null;
                        dt.Clear(); 
                        dataGridView1.Rows.Clear();
                    }
                    catch (Exception xcp)
                    {
                        MessageBox.Show(xcp.ToString());
                        tran.Rollback();
                        
                    }
                }
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ConnectionStringSettings student;
            student = ConfigurationManager.ConnectionStrings["exam"];
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = student.ConnectionString;
                cn.Open();
                using (SqlTransaction tran = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = $"delete from fees where studentid='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = $"delete from student where id='{textBox1.Text}'";
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            dataGridView1.DataSource = null;
                            dataGridView1.Rows.Clear();

                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionDB a = new ConnectionDB();
            a.conn1($"select * from student where id='{comboBox1.Text}'");
            SqlDataReader rdr = a.cmd1.ExecuteReader();//select
            while (rdr.Read())
            {
                textBox1.Text = rdr["Id"].ToString();
                textBox2.Text = rdr["Name"].ToString();
                //textBox3.Text = rdr["Fee"].ToString();
                //dateTimePicker1.Value = DateTime.Parse(rdr["date"].ToString());

                pictureBox1.Image = null;
                if (rdr["Photo"] != System.DBNull.Value)
                {
                    Byte[] byteBLOBData = new Byte[0];
                    byteBLOBData = (Byte[])((byte[])rdr["Photo"]);
                    MemoryStream ms = new MemoryStream(byteBLOBData);
                    ms.Write(byteBLOBData, 0, byteBLOBData.Length);
                    ms.Position = 0; //insert this line
                    pictureBox1.Image = Image.FromStream(ms);
                }
                pictureBox2.ImageLocation = AppDomain.CurrentDomain.BaseDirectory + rdr["stringphoto"].ToString();

            }
            a.conn1($"select BATCH,SLNO,ADDRESS,FEE,DATE from fees where studentid='{comboBox1.Text}' order by slno");

            //SqlDataReader rdr2 = a.cmd1.ExecuteReader();//select
            //int i = 0;
            //while (rdr2.Read())//until last data
            //{
            //    //textBox6.Text = rdr2["batch"].ToString();
            //    DataGridViewRow newRow = new DataGridViewRow();
            //    newRow.CreateCells(dataGridView1);
            //    newRow.Cells[0].Value = rdr2["batch"];
            //    newRow.Cells[1].Value = rdr2["slno"];
            //    newRow.Cells[2].Value = rdr2["address"];
            //    newRow.Cells[3].Value = rdr2["fee"];
            //    newRow.Cells[4].Value = rdr2["date"];
            //    //dateTimePicker1.Value = DateTime.Parse(rdr2["date"].ToString());
            //    dataGridView1.Rows.Add(newRow);
            //    i++;
            //}
            dt.Clear(); 
            textBox7.Text = pictureBox1.ImageLocation;
            textBox8.Text = pictureBox2.ImageLocation;
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            SqlDataReader rdr2 = a.cmd1.ExecuteReader();  
            dt.Load(rdr2, LoadOption.Upsert);
            dataGridView1.DataSource = dt;
            openFileDialog1.FileName = "";
          
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 a = new Form2();
            a.Show();

        }
        public static string GetComBo = "";
       
        private void button7_Click(object sender, EventArgs e)
        {
            GetComBo = comboBox1.Text;
            Form3 a = new Form3();
            a.Show();

        }
    }
}



        