# Unity Multiplayer Oyun Projesi (Lycans'a Benzer)

Bu GitHub deposu, Unity oyun motorunda FizzySteamWorks ve Mirror kullanarak geliştirilen çok oyunculu bir oyun projesini içerir.
Oyun, Lycans oyununun temel oynanışına dayanmaktadır, ancak daha gelişmiş grafikler ve daha eğlenceli mekanikler sunmayı amaçlamaktadır.

### Proje Özellikleri:

<li> Çok Oyunculu: FizzySteamWorks ve Mirror ile ağ üzerinden birden fazla oyuncu ile oynanabilir.
<li> Lycans'a Benzer Oynanış: Temel oynanış Lycans'a benzer şekilde avcı ve kurtadam rolleri arasında geçiş yapmayı içerir.
<li> Gelişmiş Grafikler: Lycans'tan daha modern ve etkileyici görseller sunar.
<li> Eğlenceli Mekanikler: Yeni oyun öğeleri ve mekanikleri ile oyunu daha ilgi çekici hale getirir.

### Başlangıç:

Bu proje hala geliştirme aşamasındadır, ancak temel oynanış mekanikleri ve ağ altyapısı tamamlanmıştır. Oyunu denemek için [PROJENİN DEMO GİT BAĞLANTISI] adresini ziyaret edebilirsiniz.

### Kod Örnekleri:

Projedeki kod, GitHub deposunda C# dilinde yazılmıştır. Aşağıda, ağ bağlantısı ve oyuncu senkronizasyonu ile ilgili bazı kod örnekleri verilmiştir:

#### Ağ Bağlantısı:

```
using Mirror;

public class NetworkManager : NetworkManager
{
    public override void OnServerConnected(NetworkConnection conn)
    {
        // Yeni bir oyuncu bağlandığında sunucu tarafından çalıştırılır
        base.OnServerConnected(conn);

        // Oyuncuyu oyuna ekle
        Player player = SpawnPlayer(conn);

        // Oyuncuya başlangıç bilgilerini gönder
        player.TargetRpc(target => target.SetInitialData(), conn);
    }
}
```
#### Oyuncu Senkronizasyonu:
```
using Mirror;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public Vector3 position;

    [SyncVar]
    public Quaternion rotation;

    void Update()
    {
        // Oyuncu hareketini ve dönüşünü sunucuya gönder
        if (isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
        }
    }
}
```
#### Örnek Görüntüler:

![1](https://github.com/Gadaffi508/Lycans-Multiplayer/assets/121219831/a9e7333f-23a1-4894-9765-05f976d1d9d2)
