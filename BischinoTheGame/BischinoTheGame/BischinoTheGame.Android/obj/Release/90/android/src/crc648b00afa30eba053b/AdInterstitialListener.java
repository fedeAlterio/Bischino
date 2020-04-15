package crc648b00afa30eba053b;


public class AdInterstitialListener
	extends com.google.android.gms.ads.AdListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAdLoaded:()V:GetOnAdLoadedHandler\n" +
			"n_onAdClosed:()V:GetOnAdClosedHandler\n" +
			"n_onAdFailedToLoad:(I)V:GetOnAdFailedToLoad_IHandler\n" +
			"";
		mono.android.Runtime.register ("BischinoTheGame.Droid.Implementations.AdInterstitialListener, BischinoTheGame.Android", AdInterstitialListener.class, __md_methods);
	}


	public AdInterstitialListener ()
	{
		super ();
		if (getClass () == AdInterstitialListener.class)
			mono.android.TypeManager.Activate ("BischinoTheGame.Droid.Implementations.AdInterstitialListener, BischinoTheGame.Android", "", this, new java.lang.Object[] {  });
	}

	public AdInterstitialListener (com.google.android.gms.ads.InterstitialAd p0)
	{
		super ();
		if (getClass () == AdInterstitialListener.class)
			mono.android.TypeManager.Activate ("BischinoTheGame.Droid.Implementations.AdInterstitialListener, BischinoTheGame.Android", "Android.Gms.Ads.InterstitialAd, Xamarin.GooglePlayServices.Ads.Lite", this, new java.lang.Object[] { p0 });
	}


	public void onAdLoaded ()
	{
		n_onAdLoaded ();
	}

	private native void n_onAdLoaded ();


	public void onAdClosed ()
	{
		n_onAdClosed ();
	}

	private native void n_onAdClosed ();


	public void onAdFailedToLoad (int p0)
	{
		n_onAdFailedToLoad (p0);
	}

	private native void n_onAdFailedToLoad (int p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
