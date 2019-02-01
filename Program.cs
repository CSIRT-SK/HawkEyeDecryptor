using System;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;

namespace HawkeyeDecrypter
{
	class MainClass
	{
		sealed class MyBinder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				return typeof(ConfigClass);
			}
		}

		public static void DumpSerializedObject(byte[] data){
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter ();
				binaryFormatter.Binder = new MyBinder();
				ConfigClass config = (ConfigClass) binaryFormatter.Deserialize(memoryStream);

				StringWriter stringWriter = new StringWriter();
				ObjectDumper.Dumper.Dump(config, "HawkEye Config", stringWriter);
				File.WriteAllText("dumped_config.txt", stringWriter.ToString());
				Console.WriteLine("Human-readable config written to dumped_config.txt\n\n");
				Console.WriteLine(stringWriter.ToString());
			}
		}

		public static byte[] DecryptAES(byte[] encrypted_data, string password){
			byte[] result;
			try
			{
				using (ICryptoTransform cryptoTransform = InitializeAES(password).CreateDecryptor())
				{
					result = cryptoTransform.TransformFinalBlock(encrypted_data, 0, encrypted_data.Length);
				}
			}
			catch (Exception ex)
			{
				result = new byte[0];
			}
			return result;
		}

		public static RijndaelManaged InitializeAES(string password) {
			byte[] salt = new byte[8];
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes (password, salt, 1000);
			RijndaelManaged rijndaelManaged = new RijndaelManaged ();
			RijndaelManaged rijndaelManaged2 = rijndaelManaged;
			rijndaelManaged2.Key = rfc2898DeriveBytes.GetBytes (rijndaelManaged.Key.Length);
			rijndaelManaged2.IV = rfc2898DeriveBytes.GetBytes (rijndaelManaged.IV.Length);
			rijndaelManaged2.Mode = CipherMode.CBC;
			rijndaelManaged2.Padding = PaddingMode.PKCS7;
			rijndaelManaged2.BlockSize = 128;
			return rijndaelManaged;
		}

		public static void DecryptConfig(byte[] encrypted_data, string password){
			RijndaelManaged rijndaelManaged = InitializeAES (password);	
			ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor();
			MemoryStream msDecrypt = new MemoryStream(encrypted_data);
			CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
			StreamReader srDecrypt = new StreamReader(csDecrypt);

			File.WriteAllText("dumped_config.bin", srDecrypt.ReadToEnd());
			Console.WriteLine("Serialized config written to dumped_config.bin");
		}

		public static void Main (string[] args)
		{
			string RCDataBase64 = "<PUT BASE64-ENCODED RCDATA HERE>";
			byte[] RCData = Convert.FromBase64String (RCDataBase64); 
			string password = "0cd08c62-955c-4bdb-aa2b-a33280e3ddce"; 
			DecryptConfig (RCData, password);
			DumpSerializedObject (DecryptAES (RCData, password));
		}			
	}

	[Serializable]
	public class ConfigClass
	{
		public ConfigClass() { }

		public string _Version;
		public string _Mutex;
		public int _Delivery;
		public string _EmailUsername;
		public string _EmailPassword;
		public string _EmailServer;
		public int _EmailPort;
		public bool _EmailSSL;
		public string _FTPServer;
		public string _FTPUsername;
		public string _FTPPassword;
		public int _FTPPort;
		public bool _FTPSFTP;
		public string _ProxyURL;
		public string _ProxySecret;
		public string _PanelURL;
		public string _PanelSecret;
		public int _LogInterval;
		public bool _PasswordStealer;
		public bool _KeyStrokeLogger;
		public bool _ClipboardLogger;
		public bool _ScreenshotLogger;
		public bool _WebCamLogger;
		public bool _SystemInfo;
		public bool _Install;
		public int _InstallLocation;
		public string _InstallFolder;
		public string _InstallFileName;
		public bool _InstallStartup;
		public bool _InstallStartupPersistance;
		public bool _HistoryCleaner;
		public bool _ZoneID;
		public bool _HideFile;
		public bool _MeltFile;
		public bool _Disablers;
		public bool _DisableTaskManager;
		public bool _DisableCommandPrompt;
		public bool _DisableRegEdit;
		public bool _ProcessProtection;
		public bool _ProcessElevation;
		public bool _AntiVirusKiller;
		public bool _BotKiller;
		public bool _AntiDebugger;
		public int _ExecutionDelay;
		public bool _FakeMessageShow;
		public string _FakeMessageTitle;
		public string _FakeMessageText;
		public int _FakeMessageIcon;
		public bool _WebsiteVisitor;
		public bool _WebsiteVisitorVisible;
		public string[] _WebsiteVisitorSites;
		public bool _WebsiteBlocker;
		public string[] _WebsiteBlockerSites;
		public bool _FileBinder;
		public string[] _FileBinderFiles;
	}
}
