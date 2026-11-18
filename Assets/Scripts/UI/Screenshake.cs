using UnityEngine;

/// <summary>
/// Scuote l'oggetto su cui si trova (tipicamente la camera).
/// Funziona anche se un altro script muove la camera ogni frame (es. CameraFollowingHook):
/// lo scuotimento viene applicato DOPO tutti gli altri LateUpdate, come piccolo spostamento
/// sopra la posizione decisa dagli altri script, e poi tolto.
/// </summary>
[DefaultExecutionOrder(10000)]
public class Screenshake : MonoBehaviour
{
    [Tooltip("Scuote anche in verticale, oltre che in orizzontale.")]
    [SerializeField] private bool shakeVertical = true;
    [Tooltip("Se spuntato lo scuotimento si attenua verso la fine (più naturale).")]
    [SerializeField] private bool fadeOut = true;

    private float intensity;
    private float duration;
    private float elapsed;

    private Vector3 appliedOffset;      // spostamento applicato nell'ultimo frame
    private Vector3 positionWithOffset; // posizione subito dopo averlo applicato

    public void TriggerShake(float intensity, float duration)
    {
        // Se c'è già uno scuotimento in corso, tieni il più forte / il più lungo rimasto.
        float remaining = Mathf.Max(0f, this.duration - elapsed);
        this.intensity = Mathf.Max(intensity, IsShaking ? this.intensity : 0f);
        this.duration = Mathf.Max(duration, remaining);
        elapsed = 0f;
    }

    private bool IsShaking => elapsed < duration;

    private void LateUpdate()
    {
        // 1) Togli lo spostamento del frame precedente, ma solo se nessun altro script
        //    ha già riposizionato l'oggetto in questo frame (altrimenti la posizione è già "pulita").
        Vector3 basePosition = transform.localPosition;
        if (appliedOffset != Vector3.zero && basePosition == positionWithOffset)
            basePosition -= appliedOffset;

        // 2) Calcola il nuovo spostamento.
        Vector3 offset = Vector3.zero;
        if (IsShaking)
        {
            elapsed += Time.deltaTime;
            float strength = intensity;
            if (fadeOut && duration > 0f)
                strength *= 1f - Mathf.Clamp01(elapsed / duration);
            offset.x = Random.Range(-1f, 1f) * strength;
            if (shakeVertical)
                offset.y = Random.Range(-1f, 1f) * strength;
            // z NON viene toccata: la camera resta alla sua profondità.
        }

        // 3) Applica.
        transform.localPosition = basePosition + offset;
        appliedOffset = offset;
        positionWithOffset = transform.localPosition;
    }

    private void OnDisable()
    {
        // Non lasciare la camera spostata se l'oggetto viene disattivato a metà scossa.
        if (appliedOffset != Vector3.zero && transform.localPosition == positionWithOffset)
            transform.localPosition -= appliedOffset;
        appliedOffset = Vector3.zero;
        elapsed = duration;
    }
}
