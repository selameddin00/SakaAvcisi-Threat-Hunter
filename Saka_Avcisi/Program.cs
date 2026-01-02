using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SakaAvcisi
{
    class Program
    {
        private static readonly string VIRUS_SIGNATURE = "X_KRAL_VIRUS_SIGNATURE_V1_X";
        private static readonly string INFECTED_PREFIX = "Enfected_";
        private static string currentExeName = string.Empty;

        static void Main(string[] args)
        {
            int eliminatedThreats = 0;
            int rescuedFiles = 0;

            try
            {
                // Kendi exe adini al (AppDomain ile kontrol icin)
                currentExeName = AppDomain.CurrentDomain.FriendlyName;

                // Baslik
                Console.WriteLine("SAKA AVCISI v1.0\n");

                // Hedef dizin secimi
                Console.Write("Taranacak dizin yolunu girin (Enter = Uygulama dizini): ");
                string? input = Console.ReadLine();

                string targetDirectory;
                if (string.IsNullOrWhiteSpace(input))
                {
                    targetDirectory = AppContext.BaseDirectory;
                }
                else
                {
                    targetDirectory = Path.GetFullPath(input.Trim().Trim('"').Trim('\''));
                }

                if (!Directory.Exists(targetDirectory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Hata: Belirtilen dizin bulunamadi: {targetDirectory}");
                    Console.ResetColor();
                    Console.WriteLine("\nCikmak icin bir tusa basin...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"\nTarama baslatiliyor: {targetDirectory}\n");

                // Tehdit tespiti ve müdahale
                eliminatedThreats = PerformThreatDetectionAndResponse(targetDirectory);

                // Sistem iyilestirme
                rescuedFiles = PerformRecoveryOperation(targetDirectory);

                // Raporlama
                Console.WriteLine("\n=== TARAMA TAMAMLANDI ===");
                Console.WriteLine($"Toplam etkisiz hale getirilen tehdit sayisi: {eliminatedThreats}");
                Console.WriteLine($"Toplam kurtarilan dosya sayisi: {rescuedFiles}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nKritik Hata: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                Console.WriteLine("\nCikmak icin bir tusa basin...");
                Console.ReadKey();
            }
        }

        static int PerformThreatDetectionAndResponse(string directory)
        {
            int eliminatedCount = 0;

            try
            {
                // Recursive olarak tum .exe dosyalarini bul
                string[] exeFiles = Directory.GetFiles(directory, "*.exe", SearchOption.AllDirectories);

                foreach (string filePath in exeFiles)
                {
                    try
                    {
                        // Kritik guvenlik kurali: Kendi calisan dosyayi tarama disi birak
                        string fileName = Path.GetFileName(filePath);
                        if (fileName.Equals(currentExeName, StringComparison.OrdinalIgnoreCase) ||
                            filePath.Equals(Process.GetCurrentProcess().MainModule?.FileName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // Binary iceriginde tehdit imzasi ara
                        if (ContainsSignature(filePath))
                        {
                            // Kirmizi renkte uyari
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"TEHDIT TESPIT EDILDI: {filePath}");
                            Console.ResetColor();

                            // Dosya aktif olarak calisiyorsa sonlandir
                            TerminateProcessIfRunning(filePath);

                            // Dosyayi diskten kalici olarak sil
                            try
                            {
                                File.Delete(filePath);
                                eliminatedCount++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Tehdit etkisiz hale getirildi: {filePath}");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"Uyari: Dosya silinemedi: {ex.Message}");
                                Console.ResetColor();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Dosya okuma hatasi (erisim reddi, kilitli dosya vb.)
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Uyari: {Path.GetFileName(filePath)} analiz edilemedi: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Tarama hatasi: {ex.Message}");
                Console.ResetColor();
            }

            return eliminatedCount;
        }

        static bool ContainsSignature(string filePath)
        {
            try
            {
                // Binary icerigini oku
                byte[] signatureBytes = Encoding.UTF8.GetBytes(VIRUS_SIGNATURE);
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Boyut kontrolu
                if (fileBytes.Length < signatureBytes.Length)
                {
                    return false;
                }

                // Binary arama
                for (int i = 0; i <= fileBytes.Length - signatureBytes.Length; i++)
                {
                    bool match = true;
                    for (int j = 0; j < signatureBytes.Length; j++)
                    {
                        if (fileBytes[i + j] != signatureBytes[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                // Binary okuma basarisiz olursa Text modda dene
                try
                {
                    string content = File.ReadAllText(filePath);
                    return content.Contains(VIRUS_SIGNATURE);
                }
                catch
                {
                    return false;
                }
            }
        }

        static void TerminateProcessIfRunning(string filePath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Tum calisan process'leri kontrol et
                Process[] processes = Process.GetProcessesByName(fileName);

                foreach (Process process in processes)
                {
                    try
                    {
                        // Process'in dosya yolu ile eslesip eslesmedigini kontrol et
                        string? processPath = process.MainModule?.FileName;
                        if (processPath != null && 
                            Path.GetFullPath(processPath).Equals(Path.GetFullPath(filePath), StringComparison.OrdinalIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Calisan process sonlandiriliyor: {process.ProcessName} (PID: {process.Id})");
                            Console.ResetColor();

                            process.Kill();
                            process.WaitForExit(5000); // 5 saniye bekle
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Process sonlandirilamadi: {ex.Message}");
                        Console.ResetColor();
                    }
                    finally
                    {
                        process?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Process kontrolu basarisiz: {ex.Message}");
                Console.ResetColor();
            }
        }

        static int PerformRecoveryOperation(string directory)
        {
            int rescuedCount = 0;

            try
            {
                // Tum dosyalari tara (alt dizinler dahil)
                string[] allFiles = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

                foreach (string filePath in allFiles)
                {
                    try
                    {
                        string fileName = Path.GetFileName(filePath);
                        string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;

                        // Enfected_ oneki ile baslayan dosyalari tespit et
                        if (fileName.StartsWith(INFECTED_PREFIX, StringComparison.OrdinalIgnoreCase))
                        {
                            // Oneki kaldirarak orijinal adina geri dondur
                            string originalName = fileName.Substring(INFECTED_PREFIX.Length);
                            string newPath = Path.Combine(directoryPath, originalName);

                            // Var olan dosya cakismalarini guvenli sekilde yonet
                            if (File.Exists(newPath))
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($"Orijinal dosya zaten mevcut, atlaniyor: {originalName}");
                                Console.ResetColor();
                                continue;
                            }

                            // Dosyayi yeniden adlandir
                            File.Move(filePath, newPath);
                            rescuedCount++;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Kurtarildi: {fileName} -> {originalName}");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Dosya kurtarilamadi: {Path.GetFileName(filePath)} - {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Kurtarma islemi hatasi: {ex.Message}");
                Console.ResetColor();
            }

            return rescuedCount;
        }
    }
}
