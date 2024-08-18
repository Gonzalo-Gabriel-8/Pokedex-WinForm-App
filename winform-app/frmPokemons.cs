using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using Dominio;
using System.Data.Odbc;

namespace winform_app
{
    public partial class frmPokemons : Form
    {
        private List<Pokemon> listaPokemons;
        public frmPokemons()
        {
            InitializeComponent();
        }

        private void frmPokemons_Load(object sender, EventArgs e)
        {
            refrescarFormulario();

            cbxCampo.Items.Add("Nùmero");
            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Descripción");
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                CargaImagen(seleccionado.UrlImagen);
            }
           
        }

        private void refrescarFormulario()
        {
            PokemonNegocio negocio = new PokemonNegocio(); //crea una instancia de PokemonNegocio
            try
            {
                listaPokemons = negocio.listar();
                dgvPokemons.DataSource = listaPokemons;
                OcultarColumnas();
                pictureBoxPokemon.Load(listaPokemons[0].UrlImagen);

            }
            catch (Exception ex)
            {   /*larnzar un mensaje de advertencia pero no dejar que el programa caiga*/

                MessageBox.Show(ex.ToString());
            }
        } 

        private void OcultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            
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

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            fmrAltaPokemon alta =new fmrAltaPokemon();

            alta.ShowDialog(); /*no poder ir a otra ventana*/

            refrescarFormulario();

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem; //le paso por parametros el objeto pokemon que voy a modificar

            fmrAltaPokemon modificar = new fmrAltaPokemon(seleccionado); //llamo al otro constructor con el parametro

            modificar.ShowDialog(); 

            refrescarFormulario();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico=false)
        { 
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿Deseas Eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                    {
                        negocio.EliminarLogico(seleccionado.Id);
                    }
                    else
                    {
                        negocio.Eliminar(seleccionado.Id);
                    }
                        
                    
                    refrescarFormulario();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool ValidarFiltro()
        {
            if (cbxCampo.SelectedIndex == -1)
            {
                MessageBox.Show("^Por favor seleccione el campo"); //valida el campo
                return true;
            }
            if (CboCriterio.SelectedIndex == -1)
            {
                MessageBox.Show("^Por favor seleccione el criterio"); //valida el criterio
                return true;
            }

            if(cbxCampo.SelectedItem.ToString()== "Nùmero")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar los numeros");
                    return true;
                }
                if (!(SoloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Ingrese solo Numeros");
                    return true;
                }
               
            }

            return false;
        }

        private bool SoloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if(!(char.IsNumber(caracter)))

                    return false;
            }

            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                if (ValidarFiltro())
                {
                    return; //cancela la ejecucion del evento
                }
                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = CboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            //Aplicar un filto sobre la listaPokemon

            List<Pokemon> ListaFiltrada;


            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3) //resetear la lista
            {
                //una suerte de forEach para evaluar si el nombre del objeto es igual al filtro que le di
                ListaFiltrada = listaPokemons.FindAll(x => x.Nombre.ToLower().Contains(filtro.ToLower()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                ListaFiltrada = listaPokemons;
            }


            dgvPokemons.DataSource = null; //una limpieza

            dgvPokemons.DataSource = ListaFiltrada;

            OcultarColumnas();
        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbxCampo.SelectedItem.ToString();
            
            if (opcion == "Nùmero")
            {
                CboCriterio.Items.Clear();
                CboCriterio.Items.Add("Mayor a");
                CboCriterio.Items.Add("Menor a");
                CboCriterio.Items.Add("Igual a");
            }
            else
            {
                CboCriterio.Items.Clear();
                CboCriterio.Items.Add("Comienza con");
                CboCriterio.Items.Add("Termina con");
                CboCriterio.Items.Add("Contiene");
            }
        }

        private void cboFiltroAvanzado_SelectedIndexChanged(object sender, EventArgs e)
        {
            

        }
    }
}
