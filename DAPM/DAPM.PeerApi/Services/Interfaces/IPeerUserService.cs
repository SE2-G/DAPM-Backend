using UtilLibrary.Interfaces;
using UtilLibrary.models;

namespace DAPM.PeerApi.Services.Interfaces
{
    public interface IPeerUserService
    {
        // Users

        public void OnGetPeerUsers();

        // invite guest user (term to denote user from other org)

        public void OnInvitePeerUser(Identity identity);

        // Resources

        public void OnGetSharedPeerResources(Identity identity);

        // proxy versions of original, that syncs actions with originating peer.

    }
}
