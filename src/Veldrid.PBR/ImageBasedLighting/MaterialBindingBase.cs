namespace Veldrid.PBR.ImageBasedLighting
{
    public abstract class MaterialBindingBase<T> where  T: struct
    {
        private readonly IUniformPool<T> _uniformPool;
        private protected uint _offset;

        protected MaterialBindingBase(IUniformPool<T> uniformPool)
        {
            _uniformPool = uniformPool;
            _offset = _uniformPool.Allocate();

        }

        public abstract void Update();

        protected void UpdateBuffer(ref T buffer)
        {
            _uniformPool.UpdateBuffer(_offset, ref buffer);
        }

        public virtual void Dispose()
        {
            //If resource set aquired via cache there is no need to dispose it.
            //ResourceSet.ResourceSet?.Dispose();
            _uniformPool.Release(_offset);
        }
    }
}