public class TargetableButton : HighlightableActivator
{
    

    public new void activate()
    {
        if (this._active)
        {
         this.deactivate();   
        }
        else
        {
            base.activate();
            this._active = true; 
        }
    }

    private new void deactivate()
    {
        base.deactivate();
        this._active = false;
    }
}