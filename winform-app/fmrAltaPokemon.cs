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
using Dominio;
using Negocio;
using System.Configuration;

namespace winform_app
{
    public partial class fmrAltaPokemon : Form
    {
        private Pokemon pokemon=null; //atrubuto inicializado en null
        private OpenFileDialog archivo=null;
        public fmrAltaPokemon()
        {
            InitializeComponent();
        }

        public fmrAltaPokemon( Pokemon pokemon)
        { //contructor modificado para pasarle por parametro un pokemon
            InitializeComponent();
            this.pokemon = pokemon; //este atrubuto privado le asigno el objeto
            Text = "Modificar Pokemon";
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio=new PokemonNegocio();

            try
            {
                if (pokemon==null)
                
                    pokemon= new Pokemon();
                
                pokemon.Numero= int.Parse(txtNumero.Text);

                pokemon.Nombre= txtNombre.Text;

                pokemon.Descripcion= txtDescripcion.Text;

                pokemon.UrlImagen= txtUrlImagen.Text;
                
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem; //capturar la valor del desplegable

                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem; //capturar la valor del desplegable

                if (pokemon.Id!= 0)
                {
                    negocio.Modificar(pokemon);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.Agregar(pokemon);

                    MessageBox.Show("Agregado Exitosamente");
                }

                //Guardo la imagen si la guardo localmente:
                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP"))) 
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["image-folder" + archivo.SafeFileName]); //copìa el archivo en la carpeta
                }

                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        /*Cargar combos desplegables*/
        private void fmrAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();

            try
            {
                //clave valor del desplegable. EL el valor con el que me voy a quedar y lo que voy a mostrar
                cboTipo.DataSource = elementoNegocio.Listar();
                cboTipo.ValueMember = "Id"; //valor clave
                cboTipo.DisplayMember = "Descripcion";//lo qque vas a mostrar

                cboDebilidad.DataSource=elementoNegocio.Listar();
                cboDebilidad.ValueMember="Id";
                cboDebilidad.DisplayMember = "Descripcion";

                if(pokemon!=null) //Si el pokemon esta distinto de null es un modificar
                {
                    txtNumero.Text=pokemon.Numero.ToString();
                    txtNombre.Text=pokemon.Nombre;
                    txtDescripcion.Text=pokemon.Descripcion;                    
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    CargaImagen(pokemon.UrlImagen); //precargar la imagen
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue=pokemon.Debilidad.Id;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            CargaImagen(txtUrlImagen.Text);
        }
        private void CargaImagen(string imagen)
        {
            try
            {
                pictureBoxPokemon.Load(imagen);
            }
            catch (Exception ex)
            {

                pictureBoxPokemon.Load("https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivo = new OpenFileDialog();

            archivo.Filter = "jpg|*.jpg;| png|*.png";

            archivo.ShowDialog();

            if(archivo.ShowDialog() == DialogResult.OK) //Permite capturar un archico
            {
                try
                {
                    txtUrlImagen.Text = archivo.FileName; //Guarda la ruta completa del archivo que estoy seleccionando

                    CargaImagen(archivo.FileName);

                    
                    

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.ToString());
                }
                 
            }
        }
    }
}
