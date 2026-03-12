using System.ComponentModel.DataAnnotations; // Digunakan untuk memberikan atribut seperti Primary Key

namespace OsisVotingAPI.Models
{
    // ========================================
    // MODEL STUDENT (TABEL SISWA)
    // ========================================
    public class Student
    {
        // Menandakan bahwa student_id adalah Primary Key di database
        [Key]
        public int student_id { get; set; }

        // NISN siswa (Nomor Induk Siswa Nasional)
        // Digunakan untuk login ke sistem voting
        public string nisn { get; set; }

        // Nama lengkap siswa
        public string name { get; set; }

        // Email siswa (opsional untuk komunikasi atau login tambahan)
        public string email { get; set; }

        // Password siswa untuk login
        // Catatan: di sistem produksi sebaiknya password di-hash
        public string password { get; set; }

        // Menandakan apakah siswa sudah melakukan voting
        // true = sudah memilih
        // false = belum memilih
        public bool has_voted { get; set; }
    }

    // ========================================
    // MODEL CANDIDATE (TABEL CALON OSIS)
    // ========================================
    public class Candidate
    {
        // Primary Key untuk kandidat
        [Key]
        public int candidate_id { get; set; }

        // Nama kandidat ketua OSIS
        public string name { get; set; }

        // URL gambar kandidat
        // Biasanya berisi link foto yang disimpan di server / cloud
        public string image_url { get; set; }

        // Deskripsi kandidat
        // Bisa berisi visi misi atau profil singkat
        public string description { get; set; }
    }

    // ========================================
    // MODEL VOTE (TABEL SUARA)
    // ========================================
    public class Vote
    {
        // Primary Key untuk data vote
        [Key]
        public int vote_id { get; set; }

        // ID siswa yang melakukan voting
        // Relasi dengan tabel Student
        public int student_id { get; set; }

        // ID kandidat yang dipilih
        // Relasi dengan tabel Candidate
        public int candidate_id { get; set; }

        // Waktu saat voting dilakukan
        // Default otomatis saat data dibuat
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}