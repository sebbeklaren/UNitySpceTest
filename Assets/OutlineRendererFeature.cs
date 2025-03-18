using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRendererFeature : ScriptableRendererFeature
{
    class OutlineRenderPass : ScriptableRenderPass
    {
        private RTHandle outlineTexture; // Use RTHandle instead of RenderTargetHandle
        private Material outlineMaterial;
        private RTHandle source; // Use RTHandle for the camera target

        public OutlineRenderPass(Material outlineMaterial)
        {
            this.outlineMaterial = outlineMaterial;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques; // Render after opaque objects
        }

        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Create a temporary render texture for the outline
            RenderingUtils.ReAllocateIfNeeded(ref outlineTexture, cameraTextureDescriptor, name: "_OutlineTexture");
            ConfigureTarget(outlineTexture);
            ConfigureClear(ClearFlag.Color, Color.black); // Clear the texture to black
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

            // Render selected objects to the outline texture
            var drawSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            var filterSettings = new FilteringSettings(RenderQueueRange.opaque, LayerMask.GetMask("Selected"));
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings);

            // Apply the outline effect (Blit using RTHandle)
            Blit(cmd, source, outlineTexture, outlineMaterial, 0);
            Blit(cmd, outlineTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (outlineTexture != null)
            {
                outlineTexture.Release(); // Release the temporary texture
            }
        }
    }

    [SerializeField] private Material outlineMaterial;
    private OutlineRenderPass outlineRenderPass;

    public override void Create()
    {
        outlineRenderPass = new OutlineRenderPass(outlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (outlineMaterial == null)
        {
            Debug.LogWarning("Outline Material is missing.");
            return;
        }

        // Get the camera target as an RTHandle
        var cameraTargetHandle = renderer.cameraColorTargetHandle;
        outlineRenderPass.Setup(cameraTargetHandle);
        renderer.EnqueuePass(outlineRenderPass);
    }
}