import { vaultTokens } from '../../brand/tokens/tokens'

export const VaultCoreBadge = ({ mode }: { mode: 'server' | 'local' }) => {
  const isLocal = mode === 'local'
  const accent = isLocal ? vaultTokens.color.green : vaultTokens.color.gold

  return (
    <div
      style={{
        backgroundColor: `${accent}15`,
        color: accent,
        border: `1px solid ${accent}40`,
        borderRadius: vaultTokens.radius.md,
        padding: `${vaultTokens.spacing[1]} ${vaultTokens.spacing[3]}`,
        fontSize: vaultTokens.typography.scale.label.fontSize,
      }}
    >
      {isLocal ? 'Local' : 'VaultWares Hosted'}
    </div>
  )
}
