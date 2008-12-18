﻿#define SISTEMA_FACTURAS_

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace Control_Inventario
{
    public partial class moduloVentasCompletarVenta : Form
    {
        public float precioPagar;
        mysql sql = new mysql();
        public ModuloVentas modVenta;

        public moduloVentasCompletarVenta()
        {
            InitializeComponent();
        }

        private void moduloVentasCompletarVenta_Load(object sender, EventArgs e)
        {
            cantidadPagar.Text = string.Format("{0:C}",precioPagar);
            cajaCantidadRecibida.Focus();
        }

        private void cajaCantidadRecibida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {             

                if (precioPagar > float.Parse(cajaCantidadRecibida.Text))
                {
                    MessageBox.Show("Cantidad Insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cajaCantidadRecibida.Clear();
                    cajaCantidadRecibida.Focus();
                }
                else
                {
                    MessageBox.Show ("Cambio: " + string.Format("{0:C}",(float.Parse(cajaCantidadRecibida.Text) - precioPagar)));
                    lbCambio.Visible = false;
                    botonCancelar.Visible = false;
                    botonFinalizar.Visible = false;
                    cantidadPagar.Visible = false;
                    lbCantidadRecibida.Visible = false;
                    cajaCantidadRecibida.Visible = false;
                    botonFinalizar.Enabled = false;
                    this.Text = "ESC Para Regresar....";

                    List<Articulo> articulosVendidos = new List<Articulo>();
                    articulosVendidos = sql.completarVenta();
                    sql.registrarGanancia(precioPagar);

#if SISTEMA_FACTURAS_
                    string deseaFactura = MessageBox.Show("Deseas Facturar esta venta?", "Factura", MessageBoxButtons.YesNo, MessageBoxIcon.Question).ToString();
                    if (deseaFactura == "Yes")
                    {
                        moduloFactura factura = new moduloFactura();
                        factura.importe = precioPagar;
                        factura.actualizarCajas();
                        factura.ShowDialog();
                    }
#endif

                    foreach (Articulo articulo in articulosVendidos)
                    {
                        cajaTicket.Text += articulo.cantidadVenta.ToString() + " " + articulo.nombre + " ( " + string.Format("{0:C}",articulo.precio) + " )" + Environment.NewLine;
                    }
                    cajaTicket.Text += Environment.NewLine + "Total: " + string.Format("{0:C}",precioPagar) + Environment.NewLine;
                    cajaTicket.Text += "Pago con: " + string.Format("{0:C}",cajaCantidadRecibida.Text) + Environment.NewLine;
                    cajaTicket.Text += "Cambio: " + string.Format("{0:C}",(float.Parse(cajaCantidadRecibida.Text) - precioPagar));
                    cajaTicket.Text += Environment.NewLine + Environment.NewLine + "Muchas Gracias por su compra!!";
                    ticket.Visible = true;
                    botonCancelar.Focus();
                }
            }
            if (e.KeyValue == 27)
            {
                modVenta.actualizarTabla();
                this.Close();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    modVenta.actualizarTabla();
                    this.Close();
                    break;                   
            }

            return base.ProcessDialogKey(keyData);
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            modVenta.actualizarTabla();
            this.Close();
        }

        private void botonFinalizar_Click(object sender, EventArgs e)
        {
            if (precioPagar > float.Parse(cajaCantidadRecibida.Text))
            {
                MessageBox.Show("Cantidad Insuficiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cajaCantidadRecibida.Clear();
                cajaCantidadRecibida.Focus();
            }
            else
            {
                lbCambio.Text = "Cambio: $" + (float.Parse(cajaCantidadRecibida.Text) - precioPagar).ToString();
                cantidadPagar.Visible = false;
                lbCantidadRecibida.Visible = false;
                cajaCantidadRecibida.Visible = false;
                botonFinalizar.Enabled = false;
                botonCancelar.Text = "Salir";

                List<Articulo> articulosVendidos = new List<Articulo>();
                articulosVendidos = sql.completarVenta();
                sql.registrarGanancia(precioPagar);

                foreach (Articulo articulo in articulosVendidos)
                {
                    cajaTicket.Text += articulo.cantidadVenta.ToString() + " " + articulo.nombre + " ( $" + articulo.precio.ToString() + " )" + Environment.NewLine;
                }
                cajaTicket.Text += Environment.NewLine + "Total: $" + precioPagar.ToString() + Environment.NewLine;
                cajaTicket.Text += "Pago con: $" + cajaCantidadRecibida.Text + Environment.NewLine;
                cajaTicket.Text += "Cambio: $" + (float.Parse(cajaCantidadRecibida.Text) - precioPagar).ToString();
                cajaTicket.Text += Environment.NewLine + Environment.NewLine + "Muchas Gracias por su compra!!";
                botonCancelar.Focus();
            }
        }
    }
}
