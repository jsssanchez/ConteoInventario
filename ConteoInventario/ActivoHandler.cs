using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ConteoInventario
{
    public class ActivoHandler
    {
        //Consulta de activo
        private Activo GetActivo(string Numero_Activo)
        {
            Activo activo;
            string conn = "Server=AGTDEVL1003;Database=conteo_inventario;Integrated Security=true;";

            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT TOP (1) * FROM Activo WHERE Numero_Activo = @Numero_Activo ;", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Numero_Activo", Numero_Activo);

                        using (var dr = cmd.ExecuteReader())
                        {
                            activo = dr.Read() ? new Activo(dr) : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message);
            }
            return activo;
        }
        //Update de activo
        private Activo UpdateActivo(string Numero_Activo)
        {
            Activo activo;
            string conn = "Server=AGTDEVL1003;Database=conteo_inventario;Integrated Security=true;";

            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("UPDATE Activo SET Estatus = 1 WHERE Numero_Activo = @Numero_Activo ;", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Numero_Activo", Numero_Activo);

                        using (var dr = cmd.ExecuteReader())
                        {
                            activo = dr.Read() ? new Activo(dr) : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message);
            }
            return activo;
        }
        //Insertar Activo
        public int InsertActivo(string Numero_Activo)
        {
            int rt = -1;
            string conn = "Server=AGTDEVL1003;Database=conteo_inventario;Integrated Security=true;";
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                using (var oTransaction = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.Transaction = oTransaction;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = $@"INSERT INTO [Activo] 
                                          (Descripcion, Estatus, Numero_Activo, Fecha_Actualizacion ) 
                                          VALUES (@Descripcion, @Estatus, @Numero_Activo, @Fecha_Actualizacion)";

                            cmd.Parameters.AddWithValue("@Descripcion", "No Definido");
                            cmd.Parameters.AddWithValue("@Estatus", 2);
                            cmd.Parameters.AddWithValue("@Numero_Activo", Numero_Activo);
                            cmd.Parameters.AddWithValue("@Fecha_Actualizacion", DateTime.Now);

                            rt = cmd.ExecuteNonQuery();
                            if (rt == 0)
                            {
                                throw new InvalidProgramException();
                            }

                            oTransaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        oTransaction.Rollback();
                    }
                    finally
                    {
                        con.Close();
                    }

                }


            }
            return rt;
        }
        //Consulta de Activo
        public string CheckActivo(string Numero_Activo)
        {
            string result = "";
            if (new ActivoHandler().GetActivo(Numero_Activo) != null)
            {
                UpdateActivo(Numero_Activo);
                result = string.Format("Activo No.{0} encontrado y marcado.\n", Numero_Activo);
            }
            else
            {
                InsertActivo(Numero_Activo);
                result = string.Format("Activo No.{0} no encontrado, fue creado como nuevo.\n", Numero_Activo);
            }

            return result;
        }
        private class Activo
        {
            int IDActivo { get; set; }
            string Descripcion { get; set; }
            int Estatus { get; set; }
            string Numero_Activo { get; set; }
            public Activo(SqlDataReader reader)
            {
                this.IDActivo = reader.GetInt32(reader.GetOrdinal("IDActivo"));
                this.Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"));
                this.Estatus = reader.GetInt32(reader.GetOrdinal("Estatus"));
                this.Numero_Activo = reader.GetString(reader.GetOrdinal("Numero_Activo"));
            }
        }

    }
}
