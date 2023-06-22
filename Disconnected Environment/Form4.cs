using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Disconnected_Environment
{
    
    public partial class Form4 : Form
    {
        private string stringConnection = "data source = DESKTOP-7VKCCI7\\REZA; database = Fakultas; user ID = sa; password = 123";
        private SqlConnection koneksi;


        public Form4()
        {
            InitializeComponent();
            koneksi = new SqlConnection(stringConnection);
            refreshfrom();
        }
        private void dataGridView()
        {
            koneksi.Open();
            string query = "SELECT id_status, nim, status_mahasiswa, tahun_masuk FROM dbo.StatusMahasiswa";
            SqlDataAdapter adapter = new SqlDataAdapter(query, koneksi);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            koneksi.Close();
        }

        private void refreshfrom()
        {
            cbxNama.Enabled = false;
            cbxStatusMahasiswa.Enabled = false;
            cbxTahunMasuk.Enabled = false;
            cbxNama.SelectedIndex = -1;
            cbxStatusMahasiswa.SelectedIndex = -1;
            cbxTahunMasuk.SelectedIndex = -1;
            txtNIM.Visible = false;
            btnSave.Enabled = true;
            btnClear.Enabled = true;
            btnAdd.Enabled = true;
        }

        private void cbTahunMasuk()
        {
            int currentYear = DateTime.Now.Year;
            int startYear = 2010;
            for (int year = startYear; year <= currentYear; year++)
            {
                cbxTahunMasuk.Items.Add(year.ToString());
            }
        }
        private void cbNama()
        {
            koneksi.Open();
            string query = "SELECT nama, nim FROM dbo.Mahasiswa WHERE NOT EXISTS(SELECT id_status FROM dbo.StatusMahasiswa WHERE StatusMahasiswa.nim = mahasiswa.nim)";
            SqlCommand command = new SqlCommand(query, koneksi);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string namaMahasiswa = reader["nama"].ToString();
                string nim = reader["nim"].ToString();
                cbxNama.Items.Add(namaMahasiswa);
                cbxNama.ValueMember = nim;
            }
            reader.Close();
            koneksi.Close();
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView();
            btnOpen.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cbxTahunMasuk.Enabled = true;
            cbxNama.Enabled = true;
            cbxStatusMahasiswa.Enabled = true;
            txtNIM.Visible = true;
            cbTahunMasuk();
            cbNama();
            btnAdd.Enabled = true;
            btnClear.Enabled = true;
            btnSave.Enabled = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            refreshfrom();
            
        }

        private void label5_Click(object sender, EventArgs e)
        {
            
        }

        private void cbxNama_SelectedIndexChanged(object sender, EventArgs e)
        {
            koneksi.Open();
            string nim = "";
            string strs = "select NIM from dbo.Mahasiswa where nama = @nm";
            SqlCommand cm = new SqlCommand(strs, koneksi);
            cm.CommandType = CommandType.Text;
            cm.Parameters.Add(new SqlParameter("@nm", cbxNama.Text));
            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                nim = dr["NIM"].ToString();
            }
            dr.Close();
            koneksi.Close();

            txtNIM.Text = nim;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            string nim = txtNIM.Text;
            string statusMahasiswa = cbxStatusMahasiswa.Text;
            string tahunMasuk = cbxTahunMasuk.Text;

            koneksi.Open();

            // Get the maximum id_status value from the table
            string getMaxIdQuery = "SELECT MAX(id_status) FROM dbo.StatusMahasiswa";
            SqlCommand getMaxIdCommand = new SqlCommand(getMaxIdQuery, koneksi);
            object maxIdResult = getMaxIdCommand.ExecuteScalar();
            int newId = (maxIdResult != DBNull.Value) ? Convert.ToInt32(maxIdResult) + 1 : 1;

            string insertQuery = "INSERT INTO dbo.StatusMahasiswa (id_status, nim, status_mahasiswa, tahun_masuk) VALUES (@idStatus, @nim, @statusMahasiswa, @tahunMasuk)";
            SqlCommand insertCommand = new SqlCommand(insertQuery, koneksi);
            insertCommand.Parameters.AddWithValue("@idStatus", newId);
            insertCommand.Parameters.AddWithValue("@nim", nim);
            insertCommand.Parameters.AddWithValue("@statusMahasiswa", statusMahasiswa);
            insertCommand.Parameters.AddWithValue("@tahunMasuk", tahunMasuk);
            insertCommand.ExecuteNonQuery();

            koneksi.Close();

            MessageBox.Show("Data Berhasil Disimpan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            refreshfrom();
            dataGridView();
        }
    }
}
