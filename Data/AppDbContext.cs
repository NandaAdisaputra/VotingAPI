using Microsoft.EntityFrameworkCore;   // Library utama Entity Framework Core untuk mengelola database
using OsisVotingAPI.Models;           // Mengakses model seperti Student, Candidate, Vote

namespace OsisVotingAPI.Data
{
    // AppDbContext adalah class yang menghubungkan aplikasi dengan database
    // Class ini mewarisi (inherit) dari DbContext milik Entity Framework
    public class AppDbContext : DbContext
    {
        // Constructor yang menerima konfigurasi koneksi database
        // options berisi konfigurasi seperti connection string SQL Server
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ================================
        // TABLE STUDENT
        // ================================

        // DbSet<Student> merepresentasikan tabel "student" di database
        // Setiap record di tabel ini akan direpresentasikan sebagai object Student
        public DbSet<Student> student { get; set; }

        // ================================
        // TABLE CANDIDATES
        // ================================

        // DbSet<Candidate> merepresentasikan tabel "candidates"
        // Berisi daftar calon ketua OSIS
        public DbSet<Candidate> candidates { get; set; }

        // ================================
        // TABLE VOTES
        // ================================

        // DbSet<Vote> merepresentasikan tabel "votes"
        // Menyimpan data siapa memilih siapa
        public DbSet<Vote> votes { get; set; }
    }
}