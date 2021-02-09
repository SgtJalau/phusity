public class TargetableButton : Activator
{
    private bool _active = false;

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