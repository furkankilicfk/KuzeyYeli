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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Disconnected Mimari Yöntemiyle veri işleme
        SqlConnection baglanti = new SqlConnection("Server=localhost; Database=KuzeyYeli;Integrated Security=true");
        //uzaktaki bilgisayara bağlanmak istiyorsanız kullanıcı adı ve parola vermemiz gerekiyor  user=sa; Pwd:123;

        //Integrated Security: Windows Authentication ile server'a bağlanmayı sağlar
        private void Form1_Load(object sender, EventArgs e)
        {
            //***disconnected mimari ile veri listelemene işlemi

            UrunListesi();
        }

        private void UrunListesi()
        {
            SqlDataAdapter adp = new SqlDataAdapter("select * from Urunler", baglanti);      //kullanıyorsak bağlantı otomatik açılır kapanır. Bağlantı connectionı otomatik olarak açacak,
                                                  /* adp.SelectCommand.Connection */                                         //buraya vermiş olduğumuz select komutunu çalıştıracak, geriye bir tablo çıktısı elde edecek,
                                                                                             //bağlantıyı kapatacak. Peki tablo çıktısını nasıl elde edip nerede tutacağız?
            DataTable dt = new DataTable(); //içerisinde tablo tutmayı sağlayan bir yapıdır. İçerisinde tablo tutar.
            adp.Fill(dt); //adaptöre dediğim şey, fill metoduyla dt'yi doldur.
                          //SqlDataAdapter sorguyu çalıştırdı, tablo çıktısı elde etti. Elde etmiş old tablo çıktısını DataTable'a dolduruyoruz. dt zaten içinde tablo barındırıyor.

            dataGridView1.DataSource = dt; //datagridview'a bir veri kaynağı vermemiz lazım ki formda içeriyi doldursun. Tabloyu old gibi datagridview'in DataSource'una bağlıyorum. (datagridview ın otomatik kolon üretme özelliği var.)
                                           //  dataGridView1.Columns["UrunID"].Visible = false;  //UrunID kolonunun görünmesini istemiyorsak çalıştıracağımız kod. Tabiiki tüm datasource u doldurduktan sonra çalıştıracağız yoksa urunid kolonu oluşmamışken görüntüsünü değiştiremeyiz hata verir.

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            SqlCommand komut = new SqlCommand();

            string adi = txtUrunAdi.Text;
            decimal fiyat = nudFiyat.Value;
            decimal stok = nudStok.Value;

            komut.CommandText = string.Format("insert Urunler(UrunAdi,Fiyat,Stok) values('{0}',{1},{2})", adi, fiyat, stok); //commandtext sql e girip çalışacak komut texti

            komut.Connection = baglanti;

            baglanti.Open();        //insert, update ve delete'te bağlantıyı elimizle açıp kapatmamız gerekiyor.

            //komut.ExecuteNonQuery();        //insert, update ve delete için kullanılır -- komutu sql e yazıp çalıştırıyor

            //eklendiğini göremiyorum ama hata da almıyorum. metodun çalışıp çalışmadğını nasıl anlayacağım? 
            //ExecuteNonQuery metodu geriye int dönderiyor. Hata veriyorsa 0, çalışıyorsa 1 dönderiyor(SQL'de execute'ta öyle). O zaman ben bunu bir int değişenine atayayım. 

            int etkilenen = komut.ExecuteNonQuery();  //Bu scopeta EN SON BAKACAĞIMIZ YER
            if (etkilenen > 0)  //etkilenen 0 dan büyük gelmişse haliyle başarılı olmuş oluyor.
            {
                MessageBox.Show("Başarılı bir şekilde kayıt eklenmiştir");
                UrunListesi();
            }
            else MessageBox.Show("Kayıt ekleme sırasında hata oluştu!!!");

            //Bu arada etkilenen 0 dan büyükse, kaydımız eklenmişse datagridviewı yenilememiz gerekiyor. Yani kaydın eklenmiş old listeyi bir daha getirmemiz göstermemiz lazım.
            //***Bunun için en yukarıda yaptığımız disconnected mimari ile veri listelemene işlemini ayrıca bir daha yapmamız gerekiyor.
            //Onu taşımak için oraya kopyalayacağıma orada yazdıklarımı bir metoda bağlarım. İstedğim yerde çağırırım. Hem load içinde hem if içinde. Her seferinde yazmama gerek kalmamış olur.


            baglanti.Close();   //baglantıyı açtım, komutu çalıştırdım, baglantıyı kapattım.

            // nvarchar tipindeki alan tek tırnakla yazılır. yoksa invalid kolumn name olur
        }

        private void btnKategoriler_Click(object sender, EventArgs e)
        {
            Kategoriler kf = new Kategoriler();
            kf.ShowDialog();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
             //datagridview'dan seçili satırı alma işlemi
             txtUrunAdi.Text = dataGridView1.CurrentRow.Cells["UrunAdi"].Value.ToString(); //dataGridView1'in, seçili satırının, hücrelerinden (hücre koleksiyondue içinde çok değer var int index veya string kolon olarak belirt),
                                                                                           //UrunAdi olan hücresinin değerini string olarak al. object gönderir hata verir tostring şart

            nudFiyat.Value = (decimal)dataGridView1.CurrentRow.Cells["Fiyat"].Value;       // decimal'a cast ettik. gelen değer decimal

            nudStok.Value = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Stok"].Value);

            txtUrunAdi.Tag = dataGridView1.CurrentRow.Cells["UrunID"].Value;  //seçili hücrenin satırındaki UrunID'yi alıyorum
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            //yazılan değerlerle yeni halini kaydı güncelleyeceğiz.

            SqlCommand komut = new SqlCommand();
            komut.CommandText = string.Format
                ("update Urunler set UrunAdi = '{0}', Fiyat = {1} , Stok = {2} where UrunID = {3}", txtUrunAdi.Text, nudFiyat.Value, nudStok.Value, txtUrunAdi.Tag);  // UrunID'si şu olan dememiz lazım
            //update işleminde sorguda kullanabilmek için, id'ye erişebilmek için seçili satıdan id'yi alıp yukarıda saklayıp daha sonra oradan çekebilmemiz lazım   
            //hücre datagridview'dan seçildiği zaman seçili hücrenin satırındaki UrunID'yi alıyorum, txtUrunAdi.Tag'e atıyorum. Kayıta bastığım zaman oradan geri alıyorum sorguya gidiyorum

            komut.Connection = baglanti;
            baglanti.Open();
            /*int etk = komut.ExecuteNonQuery(); */ //sorgular

            //EN SON!! Diyelim sorguda bir hata oldu ve if bloğuna giremedi. TRY-CATCH bloğu açmalıyız.

            try
            {
                int etk = komut.ExecuteNonQuery();
                if (etk > 0)
                {
                    MessageBox.Show("Kayıt Güncellendi");
                    UrunListesi();
                }
                else MessageBox.Show("Kayıt güncellenirken hata oluştu!!!");

                baglanti.Close();
            }
            catch (Exception ex)
            {
                baglanti.Close();       //bağlantıyı daha önce kapatsak daha mantıklı
                MessageBox.Show("Kayıt güncellenirken hata oluştu!!! " + ex.Message);  //hatanın ne olduğu mesajı da var

            }

            //if (etk > 0)
            //{
            //    MessageBox.Show("Kayıt Güncellendi");
            //    UrunListesi();
            //}
            //else MessageBox.Show("Kayıt güncellenirken hata oluştu!!!");

            //baglanti.Close();


        }

        private void sİLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow!= null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UrunID"].Value);  //seçili hücre olmayabilir (!). seçili olan hücrenin id'sini de convert edip değişkene atıyorum.
                                                                               
                SqlCommand cmd = new SqlCommand(string.Format("delete Urunler where UrunID={0}", id), baglanti);
                baglanti.Open();
                int etk = cmd.ExecuteNonQuery();
                baglanti.Close();
                if (etk > 0)
                {
                    MessageBox.Show("Kayıt Silinmiştir");
                    UrunListesi();
                }
                else MessageBox.Show("Kayıt Silinirken hata oluştu!!!");
            }
        }
    }
}
