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

namespace KuzeyYeli
{
    public partial class Kategoriler : Form
    {
        public Kategoriler()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection("Server=localhost; Database=KuzeyYeli;Integrated Security=true");

        private void Kategoriler_Load(object sender, EventArgs e)
        {
            KategoriListesi();
        }


        private void KategoriListesi()
        {
            SqlDataAdapter adp = new SqlDataAdapter("select * from Kategoriler", baglanti);      
            DataTable dt = new DataTable();
            adp.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["KategoriID"].Visible = false;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();

            string kategoriAdi = txtKategoriAdi.Text;
            string tanimi = txtTanimi.Text;


            cmd.CommandText = string.Format("insert Kategoriler(KategoriAdi,Tanimi) values('{0}','{1}')", kategoriAdi, tanimi); //commandtext sql e girip çalışacak komut texti

            cmd.Connection = baglanti;

            baglanti.Open();        

            int etk = cmd.ExecuteNonQuery();
            if (etk > 0)
            {
                MessageBox.Show("Başarılı bir şekilde kayıt eklenmiştir");
                KategoriListesi();
            }
            else MessageBox.Show("Kayıt ekleme sırasında hata oluştu!!!");

            baglanti.Close();

            //Aşağıda Herhangibir kayıt seçildiği zaman (hücre,kolon farketmez), o hücrenin bulunduğu satırdaki değerleri yukarıya dolduracağız. Ürün adı stok fiyat
            //Bunun için kullanacağımız bir event var. (CellClick hücre tıklandığında, CellContentClick hücrenin içi tıklandığında)
        }

        
    }
}
