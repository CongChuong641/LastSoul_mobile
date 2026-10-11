# 📚 TÀI LIỆU GIẢI THÍCH CODE - GAME LASTSOUL

## 📂 CẤU TRÚC TỔNG QUAN

Game chia làm 3 scenes chính:
- **MainMenu**: Màn hình menu chính
- **Gameplay**: Màn chơi chính
- **Endscene**: Màn kết thúc (Win/Lose)

---

## 🎮 1. QUẢN LÝ GAME & SCENE

### **GameManager.cs** - Quản lý toàn bộ game
**Chức năng chính:**
- Quản lý thanh Energy (năng lượng) của player
- Triệu hồi Boss khi đủ năng lượng (energyThreshold = 5)
- Quản lý Pause Menu trong gameplay
- Chuyển scene khi Win/Lose

**Các phương thức quan trọng:**
- `AddEnergy()`: Tăng năng lượng khi player giết enemy đặc biệt
- `CallBoss()`: Kích hoạt boss khi đủ 5 energy, ngừng spawn enemy thường
- `PauseGameMenu()`: Tạm dừng game (Time.timeScale = 0)
- `ResumeGame()`: Tiếp tục game
- `GameOver()`: Lưu trạng thái thua (PlayerPrefs) và chuyển sang Endscene
- `WinGame()`: Lưu trạng thái thắng và chuyển sang Endscene

**Kiến thức Unity:**
- `Time.timeScale`: Điều khiển tốc độ thời gian (0 = dừng, 1 = bình thường)
- `PlayerPrefs`: Lưu trữ dữ liệu giữa các scene
- `SceneManager.LoadScene()`: Chuyển scene

---

### **MainMenuManager.cs** - Quản lý Main Menu
**Chức năng:**
- Hiển thị màn hình menu chính
- Xử lý sự kiện nút Start Game (chuyển sang Gameplay)
- Xử lý nút Quit (thoát game)

**Lưu ý:**
- Dừng âm thanh khi vào menu
- Reset timeScale về 1 để game không bị pause

---

### **EndSceneManager.cs** - Quản lý màn kết thúc
**Chức năng:**
- Đọc PlayerPrefs để biết Win hay Lose
- Hiển thị UI tương ứng (gameOverMenu hoặc winMenu)
- Xử lý nút Retry (chơi lại), Main Menu (về menu)

**Logic:**
```csharp
isWin = PlayerPrefs.GetInt("IsWin", 0) == 1;
if (isWin) ShowWinMenu();
else ShowGameOverMenu();
```

---

### **GameUi.cs** - Quản lý UI trong Gameplay
**Chức năng:**
- Xử lý các nút trong Pause Menu
- ResumeGame(): Tiếp tục chơi
- RestartGame(): Chơi lại từ đầu
- BackToMainMenu(): Về menu chính
- QuitGame(): Thoát game

**Quan trọng:** Phải reset `Time.timeScale = 1f` trước khi chuyển scene!

---

## 👤 2. NHÂN VẬT PLAYER

### **Player.cs** - Điều khiển nhân vật chính
**Thuộc tính:**
- `moveSpeed`: Tốc độ di chuyển (5f)
- `maxHp/currentHp`: Máu tối đa và hiện tại (100)
- `hpBar`: Thanh máu hiển thị trên UI

**Các phương thức:**
- `MovePlayer()`: Di chuyển bằng WASD/Arrow keys
  - Sử dụng `Input.GetAxisRaw()` để lấy input
  - Lật sprite theo hướng di chuyển (`SpriteRenderer.flipX`)
  - Kích hoạt animation chạy (`isRun = true`)
- `TakeDame(float damage)`: Nhận sát thương, giảm máu
- `Heal(float healValue)`: Hồi máu (không vượt quá maxHp)
- `Die()`: Gọi `GameManager.GameOver()` khi hết máu

**Kiến thức Unity:**
- `Rigidbody2D.linearVelocity`: Di chuyển vật lý
- `Animator.SetBool()`: Điều khiển animation
- `Image.fillAmount`: Cập nhật thanh máu (0-1)

---

### **PlayerCollision.cs** - Xử lý va chạm của Player
**Chức năng:**
- Nhận sát thương từ đạn enemy (`EnemyBullet`)
- Thu thập USB để win game (tag `Usb`)
- Thu thập Energy (tag `Energy`)

**OnTriggerEnter2D:**
```csharp
if (tag == "EnemyBullet") -> TakeDame(10)
if (tag == "Usb") -> WinGame()
if (tag == "Energy") -> AddEnergy() + PlaySound
```

---

### **Gun.cs** - Súng của Player
**Chức năng:**
- Xoay súng theo chuột (`RotateGun()`)
- Bắn đạn khi click chuột trái (`Shoot()`)
- Nạp đạn khi click chuột phải (`Reload()`)

**Thuộc tính:**
- `maxAmmo/currentAmmo`: Đạn tối đa (50) và hiện tại
- `shotDelay`: Thời gian giữa các lần bắn (0.15s)
- `firePos`: Vị trí spawn đạn

**Logic bắn:**
```csharp
if (ClickChuotTrai && currentAmmo > 0 && Time.time > nextShot)
{
    Instantiate(bullet);
    currentAmmo--;
    PlaySound();
}
```

**Kiến thức Unity:**
- `Camera.main.ScreenToWorldPoint()`: Chuyển vị trí chuột từ màn hình sang world
- `Mathf.Atan2()`: Tính góc xoay theo chuột
- `Quaternion.Euler()`: Xoay object theo góc

---

### **PlayerBullet.cs** - Đạn của Player
**Chức năng:**
- Di chuyển về phía trước với tốc độ 25f
- Gây 15 damage khi va chạm enemy
- Tạo hiệu ứng máu (`bloodPrefabs`)
- Tự hủy sau 0.5s

---

## 👾 3. HỆ THỐNG ENEMY

### **Enemy.cs** (Abstract Class) - Lớp cha của tất cả enemy
**Chức năng chung:**
- Di chuyển về phía player (`MoveToPlayer()`)
- Lật sprite theo hướng player (`FlipEnemy()`)
- Quản lý HP và thanh máu
- Nhận sát thương và chết

**Thuộc tính:**
- `enemyMoveSpeed`: Tốc độ di chuyển
- `maxHp/currentHp`: Máu
- `enterDame`: Sát thương khi va chạm lần đầu
- `stayDame`: Sát thương liên tục khi chạm

**Tại sao dùng abstract class?**
- Để các enemy con kế thừa code chung
- Mỗi loại enemy có hành vi riêng (override)

---

### **BasicEnemy.cs** - Enemy cơ bản
**Đặc điểm:**
- Chỉ đuổi theo player và gây sát thương khi chạm
- `OnTriggerEnter2D`: Sát thương ban đầu (enterDame)
- `OnTriggerStay2D`: Sát thương liên tục (stayDame)

---

### **BossEnemy.cs** - Boss cuối cùng
**Đặc điểm đặc biệt:**
- Có 3 loại skill (random):
  1. `BanDanThuong()`: Bắn đạn thẳng về player
  2. `BanDanVongTron()`: Bắn đạn theo 8 hướng (360°)
  3. `GoiMiniEnemy()`: Triệu hồi miniEnemy để hỗ trợ
- Cooldown giữa các skill: 2 giây
- Khi chết: Spawn USB (item win game)

**Logic skill:**
```csharp
if (Time.time >= nextSkillTime)
{
    int skill = Random.Range(0, 3);
    switch (skill) { ... }
    nextSkillTime = Time.time + skillCooldown;
}
```

---

### **MiniEnemy.cs** - Enemy nhỏ (do Boss triệu hồi)
**Đặc điểm:**
- Giống BasicEnemy nhưng được boss spawn ra
- Máu và sát thương thấp hơn

---

### **HealEnemy.cs** - Enemy hồi máu
**Đặc điểm đặc biệt:**
- Khi chết: Hồi máu cho player (`healValue = 20`)
- Vẫn gây sát thương như enemy thường

**Ứng dụng:**
- Tạo chiến thuật: player có thể cố gắng giết loại này để hồi máu

---

### **EnergyEnemy.cs** - Enemy năng lượng
**Đặc điểm:**
- Khi chết: Spawn Energy object
- Player nhặt Energy để tăng thanh năng lượng
- Energy tự hủy sau 5 giây

**Mục đích:**
- Thu thập 5 energy để triệu hồi boss

---

### **ExplosionEnemy.cs** - Enemy tự nổ
**Đặc điểm nguy hiểm:**
- Khi chết: Tạo vụ nổ (`explosionPrefabs`)
- Khi chạm player: Cũng tạo vụ nổ luôn!
- Vụ nổ gây sát thương AOE (Area of Effect)

---

### **EnemySpawner.cs** - Sinh enemy tự động
**Chức năng:**
- Liên tục spawn enemy ngẫu nhiên
- Thời gian giữa mỗi lần spawn: `timeBetweenSpawns` (2s)
- Spawn tại các điểm ngẫu nhiên (`spawnPoints[]`)

**Cách hoạt động:**
```csharp
while (true)
{
    yield return new WaitForSeconds(2f);
    GameObject enemy = enemies[Random.Range(0, length)];
    Transform spawnPoint = spawnPoints[Random.Range(0, length)];
    Instantiate(enemy, spawnPoint);
}
```

**Kiến thức:**
- `Coroutine`: Hàm chạy song song, có thể delay
- `yield return`: Tạm dừng coroutine

---

### **EnemyBullet.cs** - Đạn của enemy
**Đặc điểm:**
- Di chuyển theo hướng được set (`SetMovementDirection()`)
- Tự hủy sau 5 giây
- Gây sát thương 10 cho player (xử lý trong PlayerCollision)

---

## 💥 4. CÁC OBJECT ĐẶC BIỆT

### **Explosion.cs** - Vụ nổ
**Chức năng:**
- Gây sát thương cho cả Player và Enemy (5 damage)
- Được spawn bởi ExplosionEnemy
- Tự hủy sau khi animation nổ kết thúc

**Ứng dụng:**
- Có thể dùng explosion để giết enemy khác (chiến thuật!)

---

## 🎵 5. HỆ THỐNG KHÁC

### **AudioManager.cs** - Quản lý âm thanh
**Chức năng:**
- Quản lý 3 AudioSource:
  1. `effectAudioSource`: Hiệu ứng (bắn, reload, năng lượng)
  2. `defaultAudioSource`: Nhạc nền bình thường
  3. `bossAudioSource`: Nhạc boss
  
**Các phương thức:**
- `PlayShootSound()`: Âm thanh bắn súng
- `PlayReLoadSound()`: Âm thanh nạp đạn
- `PlayEnergySound()`: Âm thanh nhặt energy
- `PlayDefaultAudio()`: Chuyển sang nhạc nền thường
- `PlayBossAudio()`: Chuyển sang nhạc boss (tắt nhạc thường)
- `StopAudioGame()`: Tắt tất cả âm thanh

**Tại sao cần nhiều AudioSource?**
- Để phát nhiều âm thanh cùng lúc (nhạc nền + hiệu ứng)

---

### **CursorManager.cs** - Quản lý con trỏ chuột
**Chức năng:**
- Đổi hình con trỏ theo hành động:
  - `cursorNormal`: Con trỏ bình thường
  - `cursorShoot`: Khi bắn (chuột trái)
  - `cursorReload`: Khi nạp đạn (chuột phải)

**Kiến thức:**
- `Cursor.SetCursor()`: Đổi hình con trỏ
- `hotspot`: Điểm "nóng" của con trỏ (vị trí click chính xác)

---

## 🎯 6. FLOW GAME (LUỒNG CHƠI)

### **Khi bắt đầu game:**
1. MainMenu scene load
2. MainMenuManager hiển thị menu
3. Player nhấn Start → Load Gameplay scene

### **Trong Gameplay:**
1. GameManager.Start() → Reset energy, bật spawn enemy
2. Player di chuyển và bắn
3. EnemySpawner liên tục spawn enemy
4. Player giết EnergyEnemy → Nhặt energy → Tăng thanh năng lượng
5. Khi đủ 5 energy → CallBoss() → Boss xuất hiện, tắt spawn
6. **Nếu player chết:** GameOver() → Load Endscene (Game Over UI)
7. **Nếu giết Boss:** WinGame() → Load Endscene (Win UI)

### **Trong Endscene:**
1. EndSceneManager.Start() → Đọc PlayerPrefs
2. Hiển thị Win hoặc Game Over menu
3. Player chọn:
   - **Retry**: Quay lại Gameplay
   - **Main Menu**: Về MainMenu
   - **Quit**: Thoát game

---

## 💡 7. KIẾN THỨC UNITY CẦN NHỚ

### **Component quan trọng:**
- `Rigidbody2D`: Vật lý 2D (di chuyển, va chạm)
- `Collider2D`: Phát hiện va chạm (Trigger hoặc Collision)
- `SpriteRenderer`: Hiển thị hình ảnh 2D
- `Animator`: Chạy animation
- `AudioSource`: Phát âm thanh
- `Image (UI)`: Hiển thị hình ảnh UI

### **Hàm callback quan trọng:**
- `Start()`: Gọi 1 lần khi object được tạo
- `Update()`: Gọi mỗi frame (~60 lần/giây)
- `OnTriggerEnter2D()`: Khi bắt đầu chạm (Collider phải có IsTrigger = true)
- `OnTriggerStay2D()`: Liên tục khi đang chạm
- `OnTriggerExit2D()`: Khi rời khỏi vùng chạm

### **Concepts:**
- **Prefab**: Template object có thể tái sử dụng
- **Instantiate()**: Tạo bản sao object
- **Destroy()**: Hủy object
- **Tag**: Nhãn để phân loại object (Player, Enemy, EnemyBullet...)
- **Scene Management**: Quản lý nhiều màn chơi

---

## 📊 8. CÂU HỎI VẤN ĐÁP CÓ THỂ GẶP

**Q1: Tại sao dùng abstract class cho Enemy?**
- Để tái sử dụng code chung (di chuyển, máu, chết)
- Các enemy con override các hành vi đặc biệt

**Q2: PlayerPrefs dùng để làm gì?**
- Lưu trạng thái Win/Lose giữa các scene
- Vì khi chuyển scene, tất cả biến bị reset

**Q3: Time.timeScale hoạt động như thế nào?**
- 0: Dừng thời gian (pause game)
- 1: Bình thường
- 2: Nhanh gấp đôi

**Q4: Coroutine khác gì với Update?**
- Update chạy mỗi frame
- Coroutine có thể delay, chạy song song

**Q5: Tại sao Boss có 3 skill ngẫu nhiên?**
- Tạo tính đa dạng, khó đoán
- Tránh nhàm chán khi chơi

**Q6: Làm sao Energy enemy biết khi nào spawn Energy?**
- Override `Die()` từ lớp cha
- Thêm logic spawn trước khi gọi `base.Die()`

**Q7: Tại sao cần EventSystem trong scene?**
- Để UI (Button, Canvas...) hoạt động
- Xử lý click chuột, touch vào UI

---

## 🔑 9. ĐIỂM NỔI BẬT CỦA CODE

### **Tính OOP tốt:**
- Kế thừa: Enemy → BasicEnemy, BossEnemy...
- Đóng gói: protected, private, public rõ ràng
- Tái sử dụng: Prefab, Component pattern

### **Tối ưu:**
- Dùng `Time.time` thay vì đếm frame
- Destroy object không cần thiết
- Audio Manager tập trung âm thanh

### **Dễ mở rộng:**
- Thêm enemy mới chỉ cần kế thừa Enemy
- Thêm skill boss chỉ cần thêm case trong switch
- UI tách biệt theo scene

---

## ✅ 10. CHECKLIST TRƯỚC KHI VẤN ĐÁP

- [ ] Hiểu flow game từ đầu đến cuối
- [ ] Giải thích được tại sao dùng abstract class
- [ ] Biết cách PlayerPrefs truyền dữ liệu giữa scenes
- [ ] Hiểu Coroutine trong EnemySpawner
- [ ] Giải thích được hệ thống skill của Boss
- [ ] Biết cách xoay súng theo chuột
- [ ] Hiểu OnTrigger vs OnCollision
- [ ] Giải thích Time.timeScale trong pause
- [ ] Biết cách UI được quản lý theo scene
- [ ] Có thể demo và giải thích từng loại enemy

---

**🎓 LỜI KHUYÊN:**
- Chạy game và debug để thấy flow thực tế
- Thử sửa các con số (damage, speed) để hiểu tác động
- Đọc Unity Documentation về các function chưa rõ
- Chuẩn bị câu trả lời "Nếu làm lại, em sẽ cải thiện gì?"

**CHÚC BẠN VẤN ĐÁP THÀNH CÔNG! 🎉**
