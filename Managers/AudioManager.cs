
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using OpenTK.Audio.OpenAL;

// I am making it static so I can just call it from wherever I need it.
public static class Sfx
{
    static ALDevice _device;
    static ALContext _context;
    static int _source;
    static int _sourcePling;
    static bool _ready;

    // Init the current device. the int[]null thing is a trial and error as I have no idea what to pass as flags.
    // I need to read the docs.
    public static void Init()
    {
        _device = ALC.OpenDevice(null);
        _context = ALC.CreateContext(_device, (int[]?)null);
        ALC.MakeContextCurrent(_context);
        AL.GenSource(out _source);
        AL.GenSource(out _sourcePling);
        _ready = true;
    }

    // don't forget to turn it off
    public static void Shutdown()
    {
        if (!_ready) return;
        AL.DeleteSource(_source);
        ALC.DestroyContext(_context);
        ALC.CloseDevice(_device);
        _ready = false;
    }

    // Call this whenever you need the shot sound
    public static void PlayShoot()
    {
        if (!_ready) return;

        const int sampleRate = 48000;
        const float duration = 0.12f;         // 120 ms
        short[] pcm = MakeShootPCM(sampleRate, duration);
        Span<byte> bytes = MemoryMarshal.AsBytes(pcm.AsSpan());
        ref byte first = ref MemoryMarshal.GetReference(bytes);
        int buffer = AL.GenBuffer();
        AL.BufferData(buffer, ALFormat.Mono16, ref first, pcm.Length * sizeof(short), sampleRate);
        AL.SourceStop(_source);
        AL.Source(_source, ALSourcei.Buffer, buffer);
        AL.SourcePlay(_source);
        AL.DeleteBuffer(buffer);
    }

    public static void PlayBoom()
    {

        if (!_ready) return;

        const int sampleRate = 48000;
        const float duration = 0.5f;         // 120 ms
        short[] pcm = MakeExplosionPCM(sampleRate, duration);
        Span<byte> bytes = MemoryMarshal.AsBytes(pcm.AsSpan());
        ref byte first = ref MemoryMarshal.GetReference(bytes);
        int buffer = AL.GenBuffer();
        AL.BufferData(buffer, ALFormat.Mono16, ref first, pcm.Length * sizeof(short), sampleRate);
        AL.SourceStop(_source);
        AL.Source(_source, ALSourcei.Buffer, buffer);
        AL.SourcePlay(_source);
        AL.DeleteBuffer(buffer);
    }
    public static void PlayPling()
    {

        if (!_ready) return;

        const int sampleRate = 48000;
        const float duration = 0.8f;         // 120 ms
        short[] pcm = MakePlingPCM(sampleRate, duration);
        Span<byte> bytes = MemoryMarshal.AsBytes(pcm.AsSpan());
        ref byte first = ref MemoryMarshal.GetReference(bytes);
        int buffer = AL.GenBuffer();
        AL.BufferData(buffer, ALFormat.Mono16, ref first, pcm.Length * sizeof(short), sampleRate);
        AL.SourceStop(_sourcePling);
        AL.Source(_sourcePling, ALSourcei.Buffer, buffer);
        AL.SourcePlay(_sourcePling);
        AL.DeleteBuffer(buffer);
    }

    // Copied from the monogame synth app. more or less.
    static short[] MakeShootPCM(int sr, float seconds)
    {
        int n = (int)(sr * seconds);
        var rnd = new Random();
        var data = new short[n];

        float baseFreq = 900f;
        float endFreq = 180f;
        float amplitudeDecay = 0.035f;
        float noiseAmt = 0.40f;
        float sineAmt = 0.65f;

        double phase = 0.0;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float k = t / seconds;
            float freq = baseFreq * MathF.Pow(endFreq / baseFreq, k);
            phase += 2.0 * Math.PI * freq / sr;

            float sine = (float)Math.Sin(phase);
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
            float env = MathF.Exp(-t / amplitudeDecay);
            float attack = MathF.Min(1f, t * 400f);
            env *= attack;

            float sample = env * (sineAmt * sine + noiseAmt * noise);
            sample = MathF.Tanh(sample * 2.2f) * 0.4f; // multiply by a volume coeff .4f
            int s = (int)(sample * short.MaxValue);
            // cap it
            s = Math.Min(s, short.MaxValue);
            s = Math.Max(s, short.MinValue);

            data[i] = (short)s;
        }
        return data;
    }


    static short[] MakeExplosionPCM(
        int sr,
        float seconds,
        int seed = 0,
        float startCutoff = 12000f,
        float endCutoff = 180f,
        float noiseGain = 1.0f,
        float rumbleGain = 0.6f,
        float crackleGain = 0.8f)
    {
        int n = Math.Max(1, (int)(sr * seconds));
        var rnd = seed == 0 ? new Random() : new Random(seed);
        var pcm = new short[n];

        // Low-rumble partials (randomized)
        int rumCount = 3;
        float[] rFreq = new float[rumCount];
        float[] rPhase = new float[rumCount];
        for (int i = 0; i < rumCount; i++)
        {
            rFreq[i] = Lerp(40f, 120f, (float)rnd.NextDouble());
            rPhase[i] = (float)(rnd.NextDouble() * Math.PI * 2);
        }

        // Envelope controls
        float attackMs = 4f;      // fast attack to avoid click
        float attack = MathF.Max(1e-4f, attackMs / 1000f);
        float decayTau = seconds * 0.55f;         // overall amplitude decay
        float rumbleTau = seconds * 0.9f;         // rumble lasts longer
        float crackleTau = 0.015f;                // each crackle burst length

        // Crackle state
        float crackleEnv = 0f;

        // One-pole low-pass state for noise
        float lp = 0f;

        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float k = seconds <= 0 ? 1f : t / seconds;

            // Exponential cutoff sweep hi -> low
            float cutoff = startCutoff * MathF.Pow(endCutoff / startCutoff, k);
            cutoff = MathF.Max(40f, cutoff);

            // Recompute one-pole coefficient for this cutoff
            // alpha = dt / (RC + dt), RC = 1 / (2πfc)
            float dt = 1f / sr;
            float rc = 1f / (2f * MathF.PI * cutoff);
            float alpha = dt / (rc + dt);

            // White noise -> time-varying low-pass
            float wn = (float)(rnd.NextDouble() * 2.0 - 1.0);
            lp += alpha * (wn - lp);

            // Global envelope (fast attack, exponential decay)
            float env = MathF.Min(1f, t / attack) * MathF.Exp(-t / MathF.Max(1e-4f, decayTau));

            // Rumble (sum sines, slow decay)
            float rum = 0f;
            float rumEnv = MathF.Exp(-t / MathF.Max(1e-4f, rumbleTau));
            for (int r = 0; r < rumCount; r++)
            {
                rPhase[r] += 2f * MathF.PI * rFreq[r] * dt;
                rum += MathF.Sin(rPhase[r]);
            }
            rum /= rumCount;

            // Occasional crackles (random spikes with quick decay)
            // Start new burst with small probability
            if (rnd.NextDouble() < 0.003) crackleEnv += 1.0f;
            crackleEnv *= MathF.Exp(-dt / MathF.Max(1e-4f, crackleTau));
            float crackle = (float)(rnd.NextDouble() * 2.0 - 1.0) * crackleEnv;

            // Mix
            float sample = env * (noiseGain * lp + crackleGain * crackle) + rumbleGain * rum * rumEnv;

            // Gentle saturation and convert to 16-bit
            sample = MathF.Tanh(sample * 1.8f) * 0.2f; // multiply by a volume coeff .2f. This is annoying FAST
            int s = (int)(sample * short.MaxValue);
            if (s > short.MaxValue) s = short.MaxValue;
            if (s < short.MinValue) s = short.MinValue;
            pcm[i] = (short)s;
        }
        return pcm;

        static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }



    static short[] MakePlingPCM(int sr, float seconds)
    {
        int n = (int)(sr * seconds);
        var data = new short[n];

        float baseFreq = 1500f;       // main pitch
        float harmonic = baseFreq * 2.01f; // slight detuned harmonic
        float amplitudeDecay = 0.15f; // longer than pew, but still short
        float sineAmt = 0.9f;         // mostly tonal
        float noiseAmt = 0.1f;        // tiny bit of strike noise

        double phase1 = 0.0;
        double phase2 = 0.0;
        var rnd = new Random();

        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;

            // envelope: fast attack, exponential decay
            float env = MathF.Exp(-t / amplitudeDecay);
            float attack = MathF.Min(1f, t * 1000f); // very fast attack
            env *= attack;

            // phase increment
            phase1 += 2.0 * Math.PI * baseFreq / sr;
            phase2 += 2.0 * Math.PI * harmonic / sr;

            // components
            float sine = (float)Math.Sin(phase1);
            float overtone = (float)Math.Sin(phase2) * 0.5f;
            float noise = (float)(rnd.NextDouble() * 2.0 - 1.0) * 0.3f;

            float sample = env * (sineAmt * (sine + overtone) + noiseAmt * noise);
            sample = MathF.Tanh(sample * 2.2f) * 0.4f;

            int s = (int)(sample * short.MaxValue);
            s = Math.Clamp(s, short.MinValue, short.MaxValue);
            data[i] = (short)s;
        }

        return data;
    }

}