using System.Text;

Console.WriteLine("Iniciando");

const int numeroDeCiclos = 20;
int contador = 0;

string rutaActual = Directory.GetCurrentDirectory();
string carpetaTwc = Path.Combine(rutaActual, "twc");
string carpetaTwcFinal = Path.Combine(rutaActual, "twc-final");

// Verificar existencia de carpetas
if (!Directory.Exists(carpetaTwc))
{
    // Creamos el directorio
    Directory.CreateDirectory(carpetaTwc);
}

if (!Directory.Exists(carpetaTwcFinal))
{
    Directory.CreateDirectory(carpetaTwcFinal);
}

// Obtener el primer archivo .twc
string[] archivos = Directory.GetFiles(carpetaTwc, "*.twc");
if (archivos.Length == 0)
{
    Console.WriteLine("No se encontraron archivos .twc en la carpeta 'twc'.");
    return;
}

// Iteraremos por cada archivo .twc
for (int i = 0; i < archivos.Length; i++)
{
    string rutaArchivoOriginal = archivos[i];
    string idPersonajeOriginal = Path.GetFileNameWithoutExtension(rutaArchivoOriginal);

    // Extraer la base del ID y el número
    (string baseIdPersonaje, int numeroIdOriginal) = ExtraerBaseId(idPersonajeOriginal);

    // Leer el archivo original una vez
    byte[] datosOriginales = File.ReadAllBytes(rutaArchivoOriginal);

    for (int j = 1; j <= numeroDeCiclos; j++)
    {
        int nuevoNumeroId = numeroIdOriginal + j;
        string nuevoIdPersonaje = baseIdPersonaje + nuevoNumeroId;

        byte[] datosModificados = ReemplazarIdEnDatos(
            datosOriginales,
            idPersonajeOriginal,
            nuevoIdPersonaje
        );

        string nuevaRutaArchivo = Path.Combine(carpetaTwcFinal, $"{nuevoIdPersonaje}.twc");
        File.WriteAllBytes(nuevaRutaArchivo, datosModificados);

        contador++;
    }

    Console.WriteLine("Finalizado");
    Console.WriteLine($"Cantidad de personajes generados: {contador}");
    Console.WriteLine("Revisa la carpeta 'twc-final'");
}

static (string baseId, int numeroId) ExtraerBaseId(string idCompleto)
{
    int length = idCompleto.Length;
    int index = length - 1;

    // Recorremos la cadena desde el final hacia el principio
    while (index >= 0 && char.IsDigit(idCompleto[index]))
    {
        index--;
    }

    // index + 1 es el inicio de la parte numérica
    string baseId = idCompleto.Substring(0, index + 1);
    string numeroStr = idCompleto.Substring(index + 1);

    // Convertimos la parte numérica a entero
    if (int.TryParse(numeroStr, out int numeroId))
    {
        return (baseId, numeroId);
    }
    else
    {
        throw new ArgumentException("El formato de la cadena no es válido.");
    }
}

static byte[] ReemplazarIdEnDatos(byte[] datos, string idAntiguo, string idNuevo)
{
    string datosHex = BitConverter.ToString(datos).Replace("-", "").ToLower();
    string idAntiguoHex = ConvertirBytesAHex(Encoding.UTF8.GetBytes(idAntiguo));
    string idNuevoHex = ConvertirBytesAHex(Encoding.UTF8.GetBytes(idNuevo));

    string datosHexModificados = datosHex.Replace(idAntiguoHex, idNuevoHex);

    return ConvertirHexAByteArray(datosHexModificados);
}

static string ConvertirBytesAHex(byte[] bytes)
{
    StringBuilder sb = new StringBuilder(bytes.Length * 2);
    foreach (byte b in bytes)
        sb.AppendFormat("{0:x2}", b);
    return sb.ToString();
}

static byte[] ConvertirHexAByteArray(string hex)
{
    int longitud = hex.Length;
    byte[] datos = new byte[longitud / 2];

    for (int i = 0; i < longitud; i += 2)
        datos[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

    return datos;
}
