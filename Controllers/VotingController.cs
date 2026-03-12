// Mengimpor library ASP.NET Core untuk membuat API Controller
using Microsoft.AspNetCore.Mvc;

// Mengimpor Entity Framework Core untuk akses database secara async
using Microsoft.EntityFrameworkCore;

// Mengimpor AppDbContext yang berfungsi sebagai penghubung aplikasi dengan database
using OsisVotingAPI.Data;

// Mengimpor model seperti Student, Vote, Candidate
using OsisVotingAPI.Models;

// Mengimpor LINQ untuk operasi query seperti Select, Where, OrderBy
using System.Linq;

namespace OsisVotingAPI.Controllers
{
    // Menandakan bahwa class ini adalah API Controller
    [ApiController]

    // Menentukan route dasar endpoint API
    // Contoh endpoint:
    // api/voting/login
    [Route("api/[controller]")]
    public class VotingController : ControllerBase
    {
        // Variabel untuk mengakses database melalui AppDbContext
        private readonly AppDbContext _context;

        // Constructor untuk dependency injection AppDbContext
        // ASP.NET otomatis memberikan instance database
        public VotingController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // API LOGIN SISWA
        // =====================================================

        // Endpoint POST
        // URL: api/voting/login
        // Digunakan untuk login siswa menggunakan NISN dan password
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            // Validasi jika data request kosong
            if (request == null)
                return BadRequest(new
                {
                    success = false,
                    message = "Data login tidak boleh kosong"
                });

            // Menghapus spasi di awal dan akhir NISN
            var nisn = request.nisn?.Trim();

            // Validasi NISN minimal 10 digit
            if (string.IsNullOrEmpty(nisn) || nisn.Length < 10)
                return BadRequest(new
                {
                    success = false,
                    message = "NISN minimal 10 digit"
                });

            // Mencari user di database berdasarkan NISN dan password
            var user = await _context.student
                .FirstOrDefaultAsync(s => s.nisn == nisn && s.password == request.password);

            // Jika user tidak ditemukan maka login gagal
            if (user == null)
                return Unauthorized(new
                {
                    success = false,
                    message = "NISN atau Password salah"
                });

            // Jika login berhasil maka kirim response success
            return Ok(new
            {
                success = true,
                message = "Login berhasil",

                // Data user yang dikirim ke client (tanpa password)
                data = new
                {
                    user.student_id,
                    user.nisn,
                    user.name,
                    user.email,

                    // Status apakah siswa sudah melakukan voting
                    user.has_voted
                }
            });
        }

        // =====================================================
        // API MENAMPILKAN CALON KETUA OSIS
        // =====================================================

        // Endpoint GET
        // URL: api/voting/candidates
        // Digunakan untuk mengambil daftar kandidat ketua OSIS
        [HttpGet("candidates")]
        public async Task<IActionResult> GetCandidates()
        {
            // Mengambil semua data kandidat dari database
            var candidates = await _context.candidates.ToListAsync();

            // Mengirim response ke client
            return Ok(new
            {
                success = true,
                message = "Data kandidat berhasil diambil",

                // Data kandidat berupa list
                data = candidates
            });
        }

        // =====================================================
        // API SUBMIT VOTING
        // =====================================================

        // Endpoint POST
        // URL: api/voting/submit-vote
        // Digunakan ketika siswa memilih kandidat
        [HttpPost("submit-vote")]
        public async Task<IActionResult> SubmitVote([FromForm] VoteRequest request)
        {
            // Validasi jika data voting kosong
            if (request == null)
                return BadRequest(new
                {
                    success = false,
                    message = "Data voting tidak boleh kosong"
                });

            // Mencari siswa berdasarkan ID
            var user = await _context.student.FindAsync(request.student_id);

            // Jika siswa tidak ditemukan
            if (user == null)
                return NotFound(new
                {
                    success = false,
                    message = "Siswa tidak ditemukan"
                });

            // Jika siswa sudah pernah voting maka tidak boleh memilih lagi
            if (user.has_voted)
                return BadRequest(new
                {
                    success = false,
                    message = "Anda sudah pernah memilih"
                });

            // Membuat object vote baru
            var vote = new Vote
            {
                // ID siswa yang melakukan voting
                student_id = request.student_id,

                // ID kandidat yang dipilih
                candidate_id = request.candidate_id
            };

            // Mengubah status siswa menjadi sudah voting
            user.has_voted = true;

            // Menambahkan data vote ke tabel votes
            await _context.votes.AddAsync(vote);

            // Menyimpan perubahan ke database
            await _context.SaveChangesAsync();

            // Response berhasil
            return Ok(new
            {
                success = true,
                message = "Voting berhasil disimpan",

                // Mengirim data vote yang baru saja dibuat
                data = vote
            });
        }

        // =====================================================
        // API MENAMPILKAN HASIL VOTING
        // =====================================================

        // Endpoint GET
        // URL: api/voting/results
        // Digunakan untuk menampilkan hasil voting
        [HttpGet("results")]
        public async Task<IActionResult> GetResults()
        {
            // Menghitung total semua vote
            var totalVotes = await _context.votes.CountAsync();

            // Mengambil semua kandidat
            var candidates = await _context.candidates.ToListAsync();

            // Membuat leaderboard hasil voting
            var leaderboard = candidates
                .Select(c =>
                {
                    // Menghitung jumlah vote untuk kandidat tertentu
                    var voteCount = _context.votes.Count(v => v.candidate_id == c.candidate_id);

                    return new
                    {
                        // ID kandidat
                        c.candidate_id,

                        // Nama kandidat
                        c.name,

                        // Jumlah vote yang diterima kandidat
                        vote_count = voteCount,

                        // Menghitung persentase vote
                        percentage = totalVotes == 0
                            ? "0%"
                            : Math.Round((double)voteCount / totalVotes * 100, 1) + "%"
                    };
                })

                // Mengurutkan dari vote terbanyak
                .OrderByDescending(x => x.vote_count)

                // Mengubah menjadi list
                .ToList();

            // Mengirim response hasil voting
            return Ok(new
            {
                success = true,
                message = "Hasil voting berhasil diambil",
                data = new
                {
                    // Total semua vote
                    total_vote = totalVotes,

                    // Data leaderboard
                    leaderboard = leaderboard
                }
            });
        }

        // =====================================================
        // MODEL REQUEST LOGIN
        // =====================================================

        // Model ini digunakan untuk menerima data login dari client
        public class LoginRequest
        {
            // NISN siswa sebagai username
            public string nisn { get; set; }

            // Password siswa
            public string password { get; set; }
        }

        // =====================================================
        // MODEL REQUEST VOTE
        // =====================================================

        // Model ini digunakan untuk menerima data voting dari client
        public class VoteRequest
        {
            // ID siswa yang melakukan voting
            public int student_id { get; set; }

            // ID kandidat yang dipilih siswa
            public int candidate_id { get; set; }
        }
    }
}